using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PSE.Customer.Configuration;
using PSE.Customer.V1.Models;
using PSE.WebAPI.Core.Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/customer/")]
    public class CustomerController : PSEController
    {
        private readonly AppSettings _config;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CustomerController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public CustomerController(IOptions<AppSettings> appSettings, IDistributedCache cache, ILogger<CustomerController> logger)
        {
            _config = (appSettings ?? throw new ArgumentNullException(nameof(appSettings))).Value;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }        

        [ProducesResponseType(typeof(LookupCustomerResponse), 200)]
        [HttpGet("lookup")]
        [AllowAnonymous]        
        public async Task<IActionResult> LookupCustomer(LookupCustomer customer)
        {
            IActionResult result = Ok(
                new LookupCustomerResponse { BPId = "1002323", Type="Residential", HasWebAccount = false}
            );

            return result;
        }

        [ProducesResponseType(typeof(IEnumerable<SecurityQuestion>), 200)]
        [HttpGet("security-questions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSecurityQuestions()
        {
            IActionResult result = Ok(new[] { new SecurityQuestion {Id = "1", Question="What is your quest?" },
                new SecurityQuestion {Id = "2", Question="What is your pet name?" },
                new SecurityQuestion {Id = "3", Question="What is your mother maiden name?" },
            });

            return result;
        }       

        [ProducesResponseType(typeof(CustomerProfile), 200)]
        [HttpGet("profile")]
        public async Task<IActionResult> GetCustomerProfile()
        {
            //get BPId from jwttoken to get customer profile
            IActionResult result = Ok(
                new CustomerProfile
                {
                    EmailAddress = "test@pse.com",
                    CustomerCredentials = new CustomerCredentials()
                    {
                        UserName = "myPseUser",
                        Password = string.Empty //never populate password on return of the object
                    },
                    MailingAddress = new Address
                    {
                        AddressLine1 = "350 110th Ave NE",
                        AddressLine2 = string.Empty,
                        City = "Bellevue",
                        State = "WA",
                        Country = "USA",
                        PostalCode = "98004-1223"
                    },
                    Phones = new List<Phone>()
                {
                    new Phone {Type=PhoneType.Cell, Number="4251234567"},
                    new Phone {Type=PhoneType.Home, Number="5251234567"},
                    new Phone {Type=PhoneType.Work, Number="6251234567", Extension="1234"}
                },
                    PrimaryPhone = PhoneType.Cell
                });

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
        public async Task<IActionResult> SaveMailingAddress(Address address)
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