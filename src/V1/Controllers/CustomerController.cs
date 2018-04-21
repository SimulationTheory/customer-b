using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using PSE.Customer.Configuration;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions;
using PSE.WebAPI.Core.Exceptions.Types;
using PSE.WebAPI.Core.Service;

namespace PSE.Customer.V1.Controllers
{
    /// <summary>
    /// Customer Controller
    /// </summary>
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/customer")]
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
        /// Lookup Customer by given customer name and account number
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /lookup?NameOnBill=BARRON%20R%20HERNANDEZ&amp;ContractAccountNumber=200000035135
        /// </remarks>
        /// <param name="lookupCustomerRequest"></param>
        /// <returns>returns LookupCustomerResponse</returns>
        /// <response code="200">Customer found</response>
        /// <response code="204">Customer not found.  (This is on purpose to avoid raising a 404 error)</response>
        [ProducesResponseType(typeof(LookupCustomerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("lookup")]
        [AllowAnonymous]
        public async Task<IActionResult> LookupCustomer(LookupCustomerRequest lookupCustomerRequest)
        {
            IActionResult result;
            _logger.LogInformation($"LookupCustomer({nameof(lookupCustomerRequest)}: {lookupCustomerRequest.ToJson()})");

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
                    _logger.LogInformation("LookupCustomer: " + response.ToJson());

                    result = Ok(response);
                }
                else
                {
                    // This is on purpose to avoid raising a 404 error. Per Peter.
                    result = StatusCode(204);
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
        /// <param name="bpId"></param>
        /// <returns>returns CustomerProfile information</returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(CustomerProfileModel), 200)]
        [HttpGet("profile")]
       public async Task<IActionResult> GetCustomerProfileAsync(long bpId = 0)
        {
            _logger.LogInformation("GetCustomerProfileAsync()");
            IActionResult result;
            
            try
            {
                if (HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues jwt))
                {
                    bpId = GetBpIdFromClaims();
                }
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
        

        /// <summary>
        /// Creates the web profile.
        /// </summary>
        /// <param name="webProfile">The web profile.</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPost("profile")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateWebProfile([FromBody] WebProfile webProfile)
        {
            IActionResult result;
            try
            {
                _logger.LogInformation($"CreateWebProfile({nameof(webProfile)}: {webProfile.ToJson()})");
                if (webProfile == null)
                {
                    return BadRequest("No profile provided");
                }

                var validBp = long.TryParse(webProfile.BPId, out var bpId);

                // Validate password,username, phone, email
                ValidateCreateProfile(webProfile, validBp);

                // Make sure the account provider exists
                var customermodel = _customerLogic.LookupCustomer(webProfile.Customer);

                // Check if username  exists
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

                //Updates Email and Phone after signup 
                var jwt = await _customerLogic.GetJWTTokenAsync(webProfile.CustomerCredentials.UserName, webProfile.CustomerCredentials.Password);
                if(!string.IsNullOrEmpty(jwt))
                {
                    await _customerLogic.PutEmailAddressAsync(jwt, webProfile.Email, bpId);
                    await _customerLogic.PutPhoneNumberAsync(jwt, webProfile.Phone, bpId);
                }
               
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
        public async Task<IActionResult> PutMailingAddressAsync([FromBody] UpdateMailingAddressRequest address)
        {
            _logger.LogInformation($"PutMailingAddressAsync({nameof(address)}: {address.ToJson()})");

            IActionResult result = BadRequest(ModelState);

            if (ModelState.IsValid)
            {
                try
                {                  

                    if (HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues jwt))
                    {
                        var bpId = GetBpIdFromClaims();

                        var model = Mapper.Map<UpdateMailingAddressModel>(address);

                        //MCF Call
                        _customerLogic.UpsertStandardMailingAddress(bpId, model, jwt);

                        //Cassandra Update
                        await _customerLogic.PutMailingAddressAsync(address, bpId);

                        result = Ok();
                    }
                    else
                    {
                        result = Unauthorized();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);

                    result = e.ToActionResult();
                } 
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
                HttpContext.Request.Headers.TryGetValue("Authorization", out var jwt);
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
                await _customerLogic.PutEmailAddressAsync(jwt, emailAddress, bpId);
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
        /// Updates the location independent phone number for logged in user
        /// </summary>
        /// <param name="phone">Phone number to write to database</param>
        /// <returns>200 if successful, 400 if address is not valid, 500 if exception</returns>
        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPut("phone")]
        public async Task<IActionResult> PutPhoneNumberAsync([FromBody] Phone phone)
        {
            _logger.LogInformation($"PutMailingAddressAsync({nameof(phone)}: {phone.ToJson()})");
            IActionResult result;

            try
            {
                HttpContext.Request.Headers.TryGetValue("Authorization", out var jwt);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bpId = GetBpIdFromClaims();
                await _customerLogic.PutPhoneNumberAsync(jwt, phone, bpId);
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
        ///  Gets BP Level Addresses of the authenticated user and store it into Redis
        /// </summary>
        /// <param name="isStandardOnly"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(GetMailingAddressesResponse), 200)]
        [HttpGet("mailing-address/{isStandardOnly}")]
        public async Task<IActionResult> GetMailingAddressesAsync(bool isStandardOnly)
        {
            _logger.LogInformation($"GetMailingAddressesAsync({nameof(isStandardOnly)}: {isStandardOnly})");

            IActionResult result;

            try
            {
                if (HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues jwt))
                {
                    var bpId = GetBpIdFromClaims();
                    var customerAddresses = await _customerLogic.GetMailingAddressesAsync(bpId, isStandardOnly,jwt);

                    if (customerAddresses != null)
                    {
                        var response = new GetMailingAddressesResponse()
                        {
                            MailingAddresses = customerAddresses
                        };

                        result = Ok(response);
                    }
                    else
                    {
                        result = NotFound();
                    }

                }
                else
                {
                    result = Unauthorized();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                result = e.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Creates interaction record for logged in user, valid priority levels are 1 (very high), 2(high),3(normal),4(low),5(very low)
        /// </summary>
        /// <param name="createCustomerInteraction"></param>
        /// <returns>returns GetCustomerInteractionResponse</returns>
        [ProducesResponseType(typeof(GetCustomerInteractionResponse), 201)]
        [HttpPost("interaction")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCustomerInteractionRecord([FromBody] CreateCustomerInteractionRequest createCustomerInteraction)
        {
            _logger.LogInformation($"CreateCustomerInteractionRecord({nameof(createCustomerInteraction)}: {createCustomerInteraction.ToJson()}");
            IActionResult result;
            Task<GetCustomerInteractionResponse> customerProfile = null;
            try
            {

                HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues jwt);
                
                customerProfile = _customerLogic.CreateCustomerInteractionRecord(createCustomerInteraction, jwt);
                                              
                result = Ok(customerProfile);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                result = e.ToActionResult();
            }

            return result;
        }

        #region Private methods

        /// <summary>
        /// Gets the Business Partner ID (bpId) from an authenticated users claims
        /// </summary>
        /// <returns>bpId as a long if it can be parsed, otherwise an exception is throws</returns>
        /// <exception cref="UnauthorizedAccessException">custom:bp is not found</exception>
        /// <exception cref="InternalServerException">custom:bp has some value other than a long value</exception>
        private long GetBpIdFromClaims()
        {
            _logger.LogInformation("LoadBpIdFromClaims()");
            _logger.LogDebug($"Claims: {User?.Claims}");
            var bp = User?.Claims?.FirstOrDefault(x => x.Type.Equals("custom:bp"))?.Value;
            if (string.IsNullOrWhiteSpace(bp))
            {
                throw new UnauthorizedAccessException($"{bp} should be Long data type");
            }

            if (!long.TryParse(bp, out var bpId))
            {
                throw new InternalServerException($"{bp} should be Long data type");
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
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Email provided doesn't met with requirements."
                });
            }

            return null;
        }
        #endregion
    }
}
