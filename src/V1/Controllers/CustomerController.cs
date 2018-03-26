using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions;
using PSE.WebAPI.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PSE.Exceptions.Core;
using System.Text.RegularExpressions;
using PSE.Customer.Extensions;

namespace PSE.Customer.V1.Controllers
{
    /// <summary>
    /// Customer Controller
    /// </summary>
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/customer/")]
    public class CustomerController : PSEController
    {
        private readonly AppSettings _config;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerLogic _customerLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="appSettings">application config information</param>
        /// <param name="cache">memory cache (currently not used)</param>
        /// <param name="logger">used for logging</param>
        /// <param name="customerLogic">contains logic for customer controller</param>
        public CustomerController(
            IOptions<AppSettings> appSettings,
            IDistributedCache cache, 
            ILogger<CustomerController> logger,
            ICustomerLogic customerLogic)
        {
            _config = (appSettings ?? throw new ArgumentNullException(nameof(appSettings))).Value;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _customerLogic = customerLogic ?? throw new ArgumentNullException(nameof(customerLogic));
        }

        /// <summary>
        /// Lookup Customer by given customer name & account number
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /lookup
        /// {
        /// "nameonbill":"test user",
        /// "contractaccountnumber": "200909090900"
        /// }
        /// </remarks>
        /// <param name="lookupCustomerRequest"></param>
        /// <returns>returns LookupCustomerResponse</returns>
        [ProducesResponseType(typeof(LookupCustomerResponse), 200)]
        [HttpGet("lookup")]
        [AllowAnonymous]
        public async Task<IActionResult> LookupCustomer(LookupCustomerRequest lookupCustomerRequest)
        {
            IActionResult result;
            _logger.LogInformation($"LookupCustomer({nameof(lookupCustomerRequest)}: {JsonConvert.SerializeObject(lookupCustomerRequest, Formatting.Indented)})");

            try
            {
                LookupCustomerModel lookupCustomerModel = await _customerLogic.LookupCustomer(lookupCustomerRequest);
                // If the cassandra call fails w/o raising an error it means that the contractAccountId wasn't found
                if (lookupCustomerModel != null)
                {
                    var response = new LookupCustomerResponse()
                    {
                        BPId = lookupCustomerModel.BPId.ToString(),
                        HasWebAccount = lookupCustomerModel.HasWebAccount,
                    };
                    _logger.LogInformation("LookupCustomer: " + JsonConvert.SerializeObject(response, Formatting.Indented));

                    result = Ok(response);
                }
                else
                {
                    result = NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to lookup customer", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Gets Customer Profile by loggedIn user
        /// </summary>
        /// <returns>returns CustomerProfile information</returns>
        [ProducesResponseType(typeof(CustomerProfileModel), 200)]
        [HttpGet("profile")]
        public async Task<IActionResult> GetCustomerProfileAsync()
        {
            _logger.LogInformation("GetCustomerProfileAsync()");
            IActionResult result;

            try
            {
                var bpId = GetBpIdFromClaims();
                var customerProfile = await _customerLogic.GetCustomerProfileAsync(bpId);

                var model = Mapper.Map<GetCustomerProfileResponse>(customerProfile);

                result = Ok(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                result = e.ToActionResult();
            }

            return result;
        }

        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPost("profile")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateWebProfile([FromBody] WebProfile webProfile)
        {
            IActionResult result;
            try
            {
                _logger.LogInformation($"CreateWebProfile({nameof(webProfile)}: {webProfile.ToJson()})");
                var validBp = long.TryParse(webProfile?.BPId, out var bpId);
                //Validate password,username, phone, email
                ValidateCreateProfile(webProfile,validBp);
                //make sure the account provider exists
                var customermodel = _customerLogic.LookupCustomer(webProfile.Customer);
                //Check if username  exists
                var usernameCheck = _customerLogic.UserNameExists(webProfile.CustomerCredentials.UserName);
                await Task.WhenAll(customermodel, usernameCheck);

                var lookupCustomerModel = customermodel.Result;
                var userExist = usernameCheck.Result;

                if (userExist)
                {
                    //409 - conflict for user name exist
                    return new BadRequestObjectResult(new ServiceError
                    {
                        Code = (int)HttpStatusCode.Conflict,
                        Message = $"The username {webProfile.CustomerCredentials.UserName} exists"
                    });
                }

                if (lookupCustomerModel == null)
                {
                    return new BadRequestObjectResult(new ServiceError
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "The Customer was not found"
                    });
                }
            
                //make sure the Bp provided and the account match
                if (bpId != lookupCustomerModel.BPId)
                {
                    return new BadRequestObjectResult(new ServiceError
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "The Contract account provided didn't match the Business Partner"
                    });
                }
                //Create profile and  save security questions
                await _customerLogic.CreateWebProfileAsync(webProfile);

                //TODO update Phone and email async when the underline services are implemented              
                result = new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                result = e.ToActionResult();
            }

            
            return result;
        }

        /// <summary>
        /// Updates postal mail address for logged in user
        /// </summary>
        /// <param name="address">Populated address to write to database</param>
        /// <returns>200 if successful, 400 if address is not valid, 500 if exception</returns>
        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPut("mailing-address")]
        public async Task<IActionResult> PutMailingAddressAsync([FromBody] AddressDefinedType address)
        {
            _logger.LogInformation($"PutMailingAddressAsync({nameof(address)}: {address})");
            IActionResult result;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bpId = GetBpIdFromClaims();
                await _customerLogic.PutMailingAddressAsync(address, bpId);
                result = Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                result = e.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Updates email address for logged in user
        /// </summary>
        /// <param name="emailAddress">Email address to write to database</param>
        /// <returns>200 if successful, 400 if address is not valid, 500 if exception</returns>
        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPut("email-address")]
        public async Task<IActionResult> PutEmailAddressAsync([FromBody] string emailAddress)
        {
            _logger.LogInformation($"PutMailingAddressAsync({nameof(emailAddress)}: {emailAddress})");
            IActionResult result;

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogInformation($"Model invalid: {emailAddress}");
                    return BadRequest(ModelState);
                }

                result = ValidateEmailAddress(emailAddress);
                if (result != null)
                {
                    _logger.LogInformation($"Email address failed validation check: {emailAddress}.");
                    return result;
                }

                var bpId = GetBpIdFromClaims();
                await _customerLogic.PutEmailAddressAsync(emailAddress, bpId);
                result = Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                result = e.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Updates phone numbers for logged in user
        /// </summary>
        /// <param name="phones">Collection of phone numbers to write to database</param>
        /// <returns>200 if successful, 400 if address is not valid, 500 if exception</returns>
        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPut("phones")]
        public async Task<IActionResult> PutPhoneNumbersAsync([FromBody] List<Phone> phones)
        {
            _logger.LogInformation($"PutMailingAddressAsync({nameof(phones)}: {phones})");
            IActionResult result;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bpId = GetBpIdFromClaims();
                await _customerLogic.PutPhoneNumbersAsync(phones, bpId);
                result = Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                result = e.ToActionResult();
            }

            return result;
        }

        #region private methods

        /// <summary>
        /// Gets the Business Partner ID (bpId) from an authenticated users claims
        /// </summary>
        /// <returns>bpId as a long if it can be parsed, otherwise a null</returns>
        private long GetBpIdFromClaims()
        {
            _logger.LogInformation("LoadBpIdFromClaims()");
            _logger.LogDebug($"Claims: {User?.Claims}");
            var bp = User?.Claims?.FirstOrDefault(x => x.Type.Equals("custom:bp"))?.Value;

            // TODO:  How should this be handled?  Return a BadRequest()?  If so, maybe at the base controller level?
            if (!long.TryParse(bp, out var bpId))
            {
                throw new InvalidRequestException($"{bp} should be Long data type");
            }

            return bpId;
        }


        private IActionResult ValidateCreateProfile(WebProfile webProfile, bool validBp)
        {

            if (!validBp)
            {
                return new BadRequestObjectResult(new ServiceError
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "The Customer Business Partner is Invalid"
                });

            }

            //check validations for username, password, email, phonenumber
            //if any fails, respond with error codes.

            if (string.IsNullOrEmpty(webProfile?.CustomerCredentials?.Password))
                return new BadRequestObjectResult(new ServiceError
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "User name doesn't met with requirements."
                });

            //Check password for Uppercase, lower case , special character and number more than 8 characters
            //@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"
            if (Regex.Match(webProfile?.CustomerCredentials?.Password,
                        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$").Length == 0)
            {
                return new BadRequestObjectResult(new ServiceError
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Password provided doesn't met with Format requirements."
                });
            }

            if (string.IsNullOrEmpty(webProfile?.CustomerCredentials?.UserName))
                return new BadRequestObjectResult(new ServiceError
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Password doesn't met with requirements."
                });

            var result = ValidateEmailAddress(webProfile.Email);
            if (result != null)
            {
                return result;
            }

            return ValidatePhone(webProfile.Phone);
        }

        private IActionResult ValidatePhone(Phone phone)
        {
            if (string.IsNullOrEmpty(phone?.Number))
                return new BadRequestObjectResult(new ServiceError
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Phone number provided doesn't met with requirements."
                });

            //Borrowed from the existing Interface web services
            //\(?\d{3}\)?-? *\d{3}-? *-?\d{4}
            if (Regex.Match(phone?.Number, @"^\(?\d{3}\)?-? *\d{3}-? *-?\d{4}$")
                    .Length == 0)
            {
                return new BadRequestObjectResult(new ServiceError
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Phone number provided doesn't met with Format requirements."
                });
            }
            return null;
        }

        private IActionResult ValidateEmailAddress(string emailAddress)
        {
            // Borrowed from the existing Interface web services
            const string emalRegEx = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

            if (string.IsNullOrEmpty(emailAddress) ||
                Regex.Match(emailAddress, emalRegEx).Length == 0)
            {
                return new BadRequestObjectResult(new ServiceError
                {
                    Code = (int) HttpStatusCode.BadRequest,
                    Message = "Email provided doesn't met with requirements."
                });
            }

            return null;
        }

        #endregion
    }
}
