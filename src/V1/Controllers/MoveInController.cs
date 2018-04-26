using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    /// API to facilitate customers moving, both new and existing customers
    /// </summary>
    /// <seealso cref="PSEController" />
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Authorize("ContractAccountValidator")]
    [Route("v{version:apiVersion}/customer")]
    public class MoveInController : PSEController
    {
        private readonly AppSettings _config;
        private readonly ILogger<MoveInController> _logger;
        private readonly IMoveInLogic _moveInLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveInController"/> class.
        /// </summary>
        /// <param name="appSettings">application config information</param>
        /// <param name="logger">The logger.</param>
        /// <param name="moveInLogic">The move in logic.</param>
        /// <exception cref="ArgumentNullException">
        /// logger
        /// or
        /// moveInLogic
        /// </exception>
        public MoveInController(
            IOptions<AppSettings> appSettings,
            ILogger<MoveInController> logger,
            IMoveInLogic moveInLogic)
        {
            _config = (appSettings ?? throw new ArgumentNullException(nameof(appSettings))).Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moveInLogic = moveInLogic ?? throw new ArgumentNullException(nameof(moveInLogic));
        }

        /// <summary>
        /// Check if the contract account provided is eligible for reconnection after successful payment/move in contract account when given reconnection flag
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// <returns></returns>
        [HttpGet("movein-status-latepayment/{contractAccountId}")]
        [ProducesResponseType(typeof(ReconnectStatusResponse), 200)]
        public IActionResult MoveInLatePayments(long contractAccountId, bool reconnectionFlag)
        {
            IActionResult result;

            try
            {
                var jwt = GetJWToken();
                _logger.LogInformation($"GetMoveInLatePayment({nameof(contractAccountId)} : {contractAccountId})");
                var response = _moveInLogic.GetMoveInLatePayment(contractAccountId, reconnectionFlag, jwt);

                result = Ok(response);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                result = e.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Initiates move in for prior obligations 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("movein-latepayment/{contractAccountId}")]
        [ProducesResponseType(typeof(LateMoveInResponse), 200)]
        public async Task<IActionResult> PostLateMoveIn([FromBody] MoveInRequest request)
        {

            IActionResult result;
            var jwt = GetJWToken();
            var bp = GetBpIdFromClaims();
           
            try
            {
                var response = new LateMoveInResponse()
                {
                    NewContractAccounts = await _moveInLogic.PostPriorMoveIn(request, bp, jwt)
                };

                result = Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                result = e.ToActionResult();
            }


            return result;
        }

        #region Start/Stop/Transfer Endpoints

        /// <summary>
        /// Search BP given both a customer first and last name, or given an organization name.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// GET /bpsearch
        /// {
        /// "FirstName":"Feng",
        /// "LastName": "Chan"
        /// }
        /// Sample Request:
        /// GET /bpsearch
        /// {
        /// "OrgName":"Company123",
        /// }
        /// </remarks>
        /// <param name="request">The business partner search criteria.<seealso cref="PSE.Customer.V1.Clients.Mcf.Request.BpSearchRequest" /></param>
        /// <returns>A BPSearchResponse object.</returns>
        [ProducesResponseType(typeof(BpSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [HttpGet("bpsearch")]
        [AllowAnonymous]
        public async Task<IActionResult> BpSearch([FromQuery]BpSearchRequest request)
        {
            _logger.LogInformation($"BpSearch({nameof(request)}: {request.ToJson()})");

            try
            {
                // null check
                if (request == null)
                {
                    throw new ArgumentNullException($"The request was empty.");
                }

                // confirm that something has been included in the request
                if ((request.FirstName == null || request.LastName == null)
                    && (request.OrgName == null))
                {
                    return BadRequest(
                        "The request must contain first and last name, or must contain organization name.");
                }

                // send call to logic class
                var searchResponse = _moveInLogic.GetDuplicateBusinessPartnerIfExists(request);

                // view result from logic class
                if (!searchResponse.MatchFound)
                {
                    _logger.LogInformation(
                        $"There is not a match for an existing Business Partner based on the information provided.");
                }

                return Ok(searchResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to search BP for a customer.", ex.Message);
                return ex.ToActionResult();
            }
        }

        /// <summary>
        /// Gets invalid movein dates (holidays and weekends) for a given date range.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /movein/invalid-movein-dates
        /// {
        /// "DateFrom":"2018-03-01T00:00:00",
        /// "DateTo": “2018-04-01T00:00:00”
        /// }
        /// </remarks>
        /// <param name="invalidMoveinDatesRequest">The invalid movein dates request.</param>
        /// <returns></returns>
        /// <response code="200">Successfully retrieved invalid movein dates.</response>
        [ProducesResponseType(typeof(GetInvalidMoveinDatesResponse), 200)]
        [HttpGet("invalid-movein-dates")]
        [AllowAnonymous]
        public async Task<IActionResult> GetInvalidMoveinDates(GetInvalidMoveinDatesRequest invalidMoveinDatesRequest)
        {
            IActionResult result;
            _logger.LogInformation($"GetInvalidMoveinDates({nameof(invalidMoveinDatesRequest)}: {invalidMoveinDatesRequest.ToJson()})");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogInformation($"Model invalid: {invalidMoveinDatesRequest}");
                    return BadRequest(ModelState);
                }

                var dates = new GetInvalidMoveinDatesResponse()
                {
                    InvalidMoveinDates = _moveInLogic.GetInvalidMoveinDates(invalidMoveinDatesRequest),
                };

                result = Ok(dates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to get invalid movein dates for DateFrom: {invalidMoveinDatesRequest.DateFrom} to DateTo: {invalidMoveinDatesRequest.DateTo}", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Creates a Business Partner 
        /// </summary>
        /// <param name="createBusinesspartnerRequest"></param>
        /// <returns></returns>

        [ProducesResponseType(typeof(BusinessPartnerResponse), StatusCodes.Status200OK)]
        [HttpPost("business-partner")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateBusinesspartner([FromBody] CreateBusinesspartnerRequest createBusinesspartnerRequest)
        {
            IActionResult result;
            _logger.LogInformation($"CreateBusinesspartner({nameof(CreateBusinesspartnerRequest)}: {createBusinesspartnerRequest.ToJson()})");

            try
            {
                var resp = await _moveInLogic.CreateBusinessPartner(createBusinesspartnerRequest);
                if (resp.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest();
                }
                var createBp = new BusinessPartnerResponse()
                {
                    BpId = resp.BpId
                };
                result = Ok(createBp);
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
                    Relationships = new List<BpRelationshipResponse>()
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
        public async Task<IActionResult> CreateBpRelationship(CreateBpRelationshipRequest bpRelationshipsRequest)
        {
            IActionResult result;
            _logger.LogInformation($"CreateBpRelationship({nameof(bpRelationshipsRequest)}: {bpRelationshipsRequest.ToJson()})");

            try
            {
                //Get the parnet Bp Id from the JWT
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
        public async Task<IActionResult> UpdateBpRelationship(CreateBpRelationshipRequest bpRelationshipsRequest)
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
        #endregion

        #region Business Partner ID Type

        /// <summary>
        /// Gets all ID types but not values for a BP
        /// </summary>
        /// <returns>returns IndentifierResponse</returns>
        [ProducesResponseType(typeof(GetBpIdTypeResponse), 200)]
        [HttpGet("bp-id-types")]
        public async Task<IActionResult> GetAllIdTypes()
        {
            IActionResult result;
            _logger.LogInformation("GetAllIdTypes()");

            try
            {
                // Get BP from user's authorization claims
                var bpId = GetBpIdFromClaims();
                result = Ok(new GetBpIdTypeResponse
                {
                    Identifiers = await _moveInLogic.GetAllIdTypes(bpId)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Get All ID types and values for BP");

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Get ID type but not value for a BP
        /// </summary>
        /// <param name="type">Represents identifier types such as last 4 SSN, drivers license number, etc.</param>
        /// <returns>returns IndentifierResponse</returns>
        [ProducesResponseType(typeof(GetBpIdTypeResponse), 200)]
        [HttpGet("bp-id-type/{type}")]
        public async Task<IActionResult> GetIdType([FromRoute] IdentifierType type)
        {
            IActionResult result;
            _logger.LogInformation($"GetIdType({nameof(type)}: {type.ToJson()})");

            try
            {
                // Get BP from user's authorization claims
                var bpId = GetBpIdFromClaims();
                result = Ok(new GetBpIdTypeResponse
                {
                    Identifiers = await _moveInLogic.GetIdType(bpId, type)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Get All ID types and values for BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Create identifier for a BP
        /// </summary>
        /// <param name="identifierRequest"></param>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(CreateBpIdTypeResponse), StatusCodes.Status200OK)]
        [HttpPost("bp-id-type")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateIdType([FromBody] IdentifierRequest identifierRequest)
        {
            IActionResult result;
            // !!! JMC - should these be logged?
            _logger.LogInformation($"CreateIdType({nameof(identifierRequest)}: {identifierRequest.ToJson()})");

            try
            {
                result = Ok(await _moveInLogic.CreateIdType(identifierRequest));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to Create IDentifier Type for a BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Update identifier for a BP
        /// </summary>
        /// <param name="identifierRequest"></param>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
        [HttpPut("bp-id-type")]
        public IActionResult UpdateIdType([FromBody] IdentifierRequest identifierRequest)
        {
            IActionResult result;
            // !!! JMC - should these be logged?
            _logger.LogInformation($"UpdateIdType({nameof(identifierRequest)}: {identifierRequest.ToJson()})");

            try
            {
                _moveInLogic.UpdateIdType(identifierRequest);
                result = Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to update IDentifier Type for a BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        /// <summary>
        /// Validate identifier for a BP (i.e. does value match what is in SAP)
        /// (Anonymous call)
        /// </summary>
        /// <param name="identifierRequest"></param>
        /// <returns>returns ValidateIdTypeResponse with Y or N</returns>
        [ProducesResponseType(typeof(ValidateIdTypeResponse), StatusCodes.Status200OK)]
        [HttpPost("bp-id-type/validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateIdType([FromBody] IdentifierRequest identifierRequest)
        {
            IActionResult result;
            // !!! JMC - should these be logged?
            _logger.LogInformation($"ValidateIdType({nameof(identifierRequest)}: {identifierRequest.ToJson()})");

            try
            {
                result = Ok(new ValidateIdTypeResponse
                {
                    PiiMatch = await _moveInLogic.ValidateIdType(identifierRequest) ? "Y" : "N"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to validate IDentifier Type for a BP", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets JWT token from the request Header. Copied from paymentarrangement
        /// </summary>
        /// <returns></returns>
        protected virtual string GetJWToken() =>
            HttpContext.Request.Headers.ContainsKey("Authorization")
                ? HttpContext.Request.Headers["Authorization"].ToString()
                : null;


        /// <summary>
        /// Gets the Business Partner ID (bpId) from an authenticated users claims
        /// </summary>
        /// <returns>bpId as a long if it can be parsed, otherwise an exception is throws</returns>
        /// <exception cref="UnauthorizedAccessException">custom:bp is not found</exception>
        /// <exception cref="InternalServerException">custom:bp has some value other than a long value</exception>
        private long GetBpIdFromClaims()
        {
            _logger.LogInformation("GetBpIdFromClaims()");
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

        #endregion
    }
}