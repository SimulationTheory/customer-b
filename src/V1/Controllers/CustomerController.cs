using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        /// <param name="appSettings"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
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
            //IActionResult result = Ok(
            //    new LookupCustomerResponse { BPId = "1002323", HasWebAccount = false}
            //);
            IActionResult result = null;
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
        public async Task<IActionResult> GetCustomerProfile()
        {
            IActionResult result = null;

            var bp = User.Claims.First(x => x.Type.Equals("custom:bp")).Value;

            if(!long.TryParse(bp, out var bpId))
            {
                throw new Exception($"{bp} should be Long data type");
            }

            try
            {
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
        public async Task<IActionResult> CreateWebProfile(WebProfile webProfile)
        {
            //check validations for username, password, email, phonenumber
            //if any fails, respond with error codes.
            //409 - conflict for user name
            if (string.IsNullOrEmpty(webProfile?.CustomerCredentials?.Password))
                return new BadRequestObjectResult(new ServiceError
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Password doesn't met with requirements."
                });

            return new OkResult();
        }

        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPut("mailing-address")]
        public async Task<IActionResult> SaveMailingAddress(AddressDefinedType address)
        {
            //This is an authorized call
            //Get BPId from claims to update mailing address
            IActionResult result = Ok();
            return result;
        }

        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPut("email-address")]
        public async Task<IActionResult> SaveEmailAddress(string emailAddress)
        {
            //This is an authorized call
            //Get BPId from claims to update email address
            IActionResult result = Ok();
            return result;
        }

        [ProducesResponseType(typeof(OkResult), 200)]
        [HttpPut("phones")]
        public async Task<IActionResult> SavePhoneNumbers(List<Phone> phones)
        {
            //This is an authorized call
            //Get BPId from claims to update phone numbers
            IActionResult result = Ok();
            return result;
        }
    }
}