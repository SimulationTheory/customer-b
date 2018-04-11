using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Service;

namespace PSE.Customer.V1.Controllers
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/customer/")]
    public class MoveInController : PSEController
    {
        private readonly ILogger<MoveInController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveInController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">logger</exception>
        public MoveInController(ILogger<MoveInController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Mock Endpoint for Latepayments movein 
        /// 
        /// A    "FirstIp" : "286.000", (first installment which is 50% of (B) ie total deposit ) 
        ///B    "DepositAmount" : "572.000", (total deposit amount) 
        ///C    "ReconAmount" : "70.000", (reconnection fees) 
        ///D    "MinPayment" : "356.000", (Item A + B)
        ///E    "IncPayment" : "320.000",  (Payment which have been made)
        ///F    "EligibleRc" : "", (If this is ‘X’ it means customer has made thresh-hold payment and s/he is eligible for reconnection)
        ///If ReconnectFlag eq 'X', it will initiate reconnection
        /// </summary>
        /// <param name="reconnectFlag">If ReconnectFlag eq 'X', it will initiate reconnection</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(MoveInLatePaymentsResponse), 200)]
        [Authorize("ContractAccountValidator")]
        [HttpGet("movein-status-latepayment/{contractAccountId}")]
        public async Task<IActionResult> MoveInLatePayments()
        {
            IActionResult result = Ok(new MoveInLatePaymentsResponse()
            {
                FirstIp = 286.00m,
                EligibleRc = true,
                AccountNo = 200028750178,
                ReconnectFlag = false,
                PriorObligationAccount = 200026140646,
                DepositAmount = 572.00m,
                ReconAmount = 70.00m,
                MinPayment = 356,
                IncPayment = 320.00m,
                AccountType = "RES",
                ReasonCode = string.Empty,
                Reason = string.Empty
            });

            return result;
        }

        /// <summary>
        /// Mock Endpoint for Latepayments movein Put, initiates movein and creates a new contract account for obligated account. 
        /// When user pays minimum/all outstanding balance, user move-in status shows "EligibleRc" to true.
        /// Call this method by setting reconnectFlag to activate the service.
        /// </summary>
        /// <param name="reconnectFlag"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(MoveInLatePaymentsResponse), 200)]
        [Authorize("ContractAccountValidator")]
        [HttpPut("movein-latepayment/{contractAccountId}")]
        public async Task<IActionResult> MoveInLatePayments([FromQuery]bool reconnectFlag)
        {
            IActionResult result = Ok(new MoveInLatePaymentsResponse()
            {

                FirstIp = 286.00m,
                EligibleRc = null,
                AccountNo = 200028750178,
                ReconnectFlag = false,
                PriorObligationAccount = 200026140646,
                DepositAmount = 572.00m,
                ReconAmount = 70.00m,
                MinPayment = 356,
                IncPayment = 320.00m,
                AccountType = "RES",
                ReasonCode = string.Empty,
                Reason = string.Empty
            });

            return result;
        }

        #region Start/Stop/Transfer Endpoints
        /// <summary>
        /// Search BP given customer firstname, MiddleName and Last Name
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /bpsearch
        /// {
        /// "FirstName":"Feng",
        /// "LastName": "Chan"
        /// }
        /// </remarks>
        /// <param name="bpSearchRequest"></param>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(BPSearchResponse), 200)]
        [HttpGet("bpsearch")]
        [AllowAnonymous]
        public async Task<IActionResult> BpSearch(BPSearchRequest bpSearchRequest)
        {
            IActionResult result;
            _logger.LogInformation($"BpSearch({nameof(bpSearchRequest)}: {bpSearchRequest.ToJson()})");

            try
            {
                //TODO remove below Mock data
                var bpsearchresponse = new BPSearchResponse()
                {
                    BPId = "1222345789",
                    FirstName = "Feng",
                    LastName = "Chan",
                    MatchPercent = "100"
                };
                result = Ok(bpsearchresponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to search BP for a customer", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// /Get Holidays in a given date range
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /holidays-in-daterange
        /// {
        /// "DateFrom":"2018-03-01T00:00:00",
        /// "DateTo": "2018-04-01T00:00:00"
        /// }
        /// </remarks>
        /// <param name="holidaysInDaterangerequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(HolidaysInDaterangeResponse), 200)]
        [HttpGet("holidays-in-daterange")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHolidaysInDaterange(HolidaysInDaterangeRequest holidaysInDaterangerequest)
        {
            IActionResult result;
            _logger.LogInformation($"GetHolidaysInDaterange({nameof(holidaysInDaterangerequest)}: {holidaysInDaterangerequest.ToJson()})");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogInformation($"Model invalid: {holidaysInDaterangerequest}");
                    return BadRequest(ModelState);
                }
                //TODO remove Mock
                //Mock Holiday Dates
                var dates = new List<DateTime>();

                for (var dt = holidaysInDaterangerequest.DateFrom; dt <= holidaysInDaterangerequest.DateTo; dt = dt.AddDays(1))
                {
                    dates.Add(dt);
                }

                var holidayranges = new HolidaysInDaterangeResponse()
                {
                    Holidays = dates
                };
                result = Ok(holidayranges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Holidays in a given datrange", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Creates a Business Partner 
        /// </summary>
        /// <param name="createBusinesspartnerRequest"></param>
        /// <returns></returns>

        [ProducesResponseType(typeof(CreateBusinesspartnerResponse), 200)]
        [HttpPost("business-partner")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateBusinesspartner([FromBody] CreateBusinesspartnerRequest createBusinesspartnerRequest)
        {
            IActionResult result;
            _logger.LogInformation($"CreateBusinesspartner({nameof(CreateBusinesspartnerRequest)}: {createBusinesspartnerRequest.ToJson()})");

            try
            {
                //TODO remove below Mock data
                var bpcreateresponse = new CreateBusinesspartnerResponse()
                {
                   BpId = "10000000001",
                   Addressnumber = "0000041221"

                };
                result = Ok(bpcreateresponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create BP for a customer", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Get all bp relationsships
        /// </summary>
        /// <param name="bpRelationshipsRequest"></param>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(BpRelationshipsResponse), 200)]
        [HttpGet("bp-relationships")]
        public async Task<IActionResult> GetAllBpRelationships()
        {
            IActionResult result;

            try
            {
                //get the Bp from the JWT
                //TODO remove below Mock data
                var bprelationshipsresponse = new BpRelationshipsResponse()
                {
                    BpId = "1000000369",
                    RelationShips = new List<BpRelationshipResponse>()
                    {
                       new BpRelationshipResponse(){BpIdParent ="1000000369", BpId = "1200000695", Relationshipcategory ="BUR001" },
                       new BpRelationshipResponse(){BpIdParent ="1000000369", BpId = "1200000698", Relationshipcategory ="CRMM02" },
                    }
                };
                result = Ok(bprelationshipsresponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get all bp relationsships BP for a customer", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Create a Bp relationship
        /// </summary>
        /// <param name="bpRelationshipsRequest"></param>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(BpRelationshipResponse), 200)]
        [HttpPost("bp-relationship")]
        public async Task<IActionResult> CreateBpRelationship(BpRelationshipRequest bpRelationshipsRequest)
        {
            IActionResult result;
            _logger.LogInformation($"CreateBpRelationship({nameof(bpRelationshipsRequest)}: {bpRelationshipsRequest.ToJson()})");

            try
            {
                //Get the parnet Bp Id from the JWt
                //TODO remove below Mock data
                var bprelationshipsresponse = new BpRelationshipResponse()
                {
                    BpIdParent = "120000047",
                    BpId = "1200000805",
                    Relationshipcategory = "ZCOCU"
                };
                result = Ok(bprelationshipsresponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to CreateBpRelationship for BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Update an existing Bp relationship
        /// </summary>
        /// <param name="bpRelationshipsRequest"></param>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(BpRelationshipResponse), 200)]
        [HttpPut("bp-relationship")]
        public async Task<IActionResult> UpdateBpRelationship(BpRelationshipRequest bpRelationshipsRequest)
        {
            IActionResult result;
            _logger.LogInformation($"CreateBpRelationship({nameof(bpRelationshipsRequest)}: {bpRelationshipsRequest.ToJson()})");

            try
            {
                
                result = Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Update Bp Relationship for BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        #region Business Partner ID Type

        /// <summary>
        /// Get All ID types and values for a Given BP
        /// </summary>
        /// <returns>returns IndentifierResponse</returns>
        [ProducesResponseType(typeof(IndentifierResponse), 200)]
        [HttpGet("bp-id-types")]
        public async Task<IActionResult> GetAllIdTypes()
        {
            IActionResult result;
            try
            {
                //Get Bp from JWt
                //TODO remove below Mock data
                var identifieResponse = new IndentifierResponse()
                {
                    Identifiers = new List<IdentifierModel>()
                    {
                        new IdentifierModel(){IdentifierType = IdentifierType.ZDOB, IdentifierValue="02/02/1978"},
                        new IdentifierModel(){IdentifierType = IdentifierType.ZLAST4, IdentifierValue="1010"},
                        new IdentifierModel(){IdentifierType = IdentifierType.ZDNAC, IdentifierValue="X"}
                    }
                };
                result = Ok(identifieResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Get All ID types and values for BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Get ID type and value for a Given BP
        /// </summary>         
        /// <returns>returns IndentifierResponse</returns>
        [ProducesResponseType(typeof(IndentifierResponse), 200)]
        [HttpGet("bp-id-type/{type}")]      
        public async Task<IActionResult> GetIdType(IdentifierType type)
        {
            IActionResult result;
            try
            {
                //Get Bp from JWt
                //TODO remove below Mock data
                var identifieResponse = new IndentifierResponse()
                {
                    Identifiers = new List<IdentifierModel>()
                    {
                        new IdentifierModel(){IdentifierType = IdentifierType.ZDOB, IdentifierValue="02/02/1978"}
                    }
                };
                result = Ok(identifieResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Get All ID types and values for BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Create Identifier type for a bp
        /// </summary>
        /// <param name="identifierRequest"></param>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
        [HttpPost("bp-id-type")]
        public async Task<IActionResult> CreateIDType(IdentifierRequest identifierRequest)
        {
            IActionResult result;
            _logger.LogInformation($"CreateIDType({nameof(identifierRequest)}: {identifierRequest.ToJson()})");

            try
            {

                result = Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Create IDentifier Type for a BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Create Identifier type for a bp
        /// </summary>
        /// <param name="identifierRequest"></param>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
        [HttpPut("bp-id-type")]
        public async Task<IActionResult> UpdateIDType(IdentifierRequest identifierRequest)
        {
            IActionResult result;
            _logger.LogInformation($"UpdateIDType({nameof(identifierRequest)}: {identifierRequest.ToJson()})");

            try
            {

                result = Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to update IDentifier Type for a BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        #endregion

        #endregion
    }
}