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
        /// Check if the contract account provided in MCF is eligible for reconnection after successful 
        /// payment/move in contract account when given reconnection flag.
        /// </summary>
        /// <param name="contractAccountId"></param>
        /// /// <param name="reconnectionFlag"></param>
        /// <returns></returns>
        [HttpGet("movein-status-latepayment/{contractAccountId}")]
        [ProducesResponseType(typeof(ReconnectStatusResponse), 200)]
        public IActionResult MoveInLatePayments(long contractAccountId, bool reconnectionFlag)
        {
            IActionResult result;

            try
            {
                var jwt = GetJWToken();
                var bp = GetBpIdFromClaims();
                _logger.LogInformation($"GetMoveInLatePayment({nameof(contractAccountId)} : {contractAccountId})");
                var response = _moveInLogic.GetMoveInLatePaymentAsync(contractAccountId, bp, reconnectionFlag, jwt);

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
        /// Clean move in in MCF.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("movein/{contractAccountId}")]
        [ProducesResponseType(typeof(CleanMoveInResponse), 200)]
        public async Task<IActionResult> PostCleanMoveIn([FromBody] CleanMoveInRequest request)
        {

            IActionResult result;

            try
            {
                var jwt = GetJWToken();
                var response = await _moveInLogic.PostCleanMoveIn(request, jwt);

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
        /// Initiates move in for prior obligations in MCF.
        /// </summary>
        /// <param name="request">The request</param>
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
        /// Search BP given both a customer first and last name, or given an organization name in MCF.
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
        /// <param name="request">The business partner search criteria.<seealso cref="PSE.Customer.V1.Models.BpSearchModel" /></param>
        /// <returns>A business partner with associated set of identifiers, or a reason why no match was found.</returns>
        [ProducesResponseType(typeof(BpSearchModel), StatusCodes.Status200OK)]
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

                // confirm that something valid has been included in the request
                if ((request.FirstName == null || request.LastName == null)
                    && (request.OrgName == null))
                {
                    return BadRequest("The request must contain first and last name, or must contain organization name.");
                }

                // send call to logic class
                var searchResponse = await _moveInLogic.GetDuplicateBusinessPartnerIfExists(request);

                // view result from logic class
                if (!searchResponse.MatchFound)
                {
                    _logger.LogInformation($"There is not a match for an existing Business Partner based on the information provided.");
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
        /// Posts a cancel move-in request to a user account in MCF.
        /// </summary>
        /// <param name="cancelRequest">The request body, including a Contract Id to cancel.</param>
        /// <returns>An action result.</returns>
        [ProducesResponseType(typeof(CancelMoveInResponse), StatusCodes.Status200OK)]
        [HttpPost("cancelmovein")]
        public async Task<IActionResult> CancelMoveIn([FromBody]CancelMoveInRequest cancelRequest)
        {
            _logger.LogInformation($"CancelMoveIn({nameof(cancelRequest)}): {cancelRequest.ToJson()}");

            try
            {
                var result = await _moveInLogic.PostCancelMoveIn(cancelRequest);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to cancel move-in for contract.", e.Message);
                return e.ToActionResult();
            }
        }

        /// <summary>
        /// Gets invalid movein dates (holidays and weekends) for a given date range in MCF.
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
        /// Creates a Business Partner in MCF.
        /// </summary>
        /// <param name="createBusinesspartnerRequest">the request</param>
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
        /// Get all bp relationsships from MCF.
        /// </summary>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(BpRelationResponse), StatusCodes.Status200OK)]
        [HttpGet("bp-relationships")]       
        public async Task<IActionResult> GetAllBpRelationships([FromQuery] string tenantBpId)
        {
            IActionResult result;

            try
            {
                //get the Bp from the JWT              
                var bpId = GetBpIdFromClaims();
                var jwt = GetJWToken();
                var bpRelationParam = new BpRelationshipRequestParam()
                {
                    LoggedInBp = bpId.ToString(),
                    Jwt = jwt,
                    TenantBp = tenantBpId

                };
                var resp = await _moveInLogic.GetBprelationships(bpRelationParam);
                var bpRelation = MapToBpRelation(resp);
                result = Ok(bpRelation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get all bp relationsships BP for a customer", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        

        /// <summary>
        /// Delete bp relationsship for the given bp in MCF.
        /// </summary>
        /// <returns>returns BPSearchResponse</returns>
        [ProducesResponseType(typeof(BpRelationshipUpdateResponse), StatusCodes.Status200OK)]
        [HttpDelete("contact/{bp}")]
        public async Task<IActionResult> DeleteCustomerContact(string bp)
        {
            IActionResult result;

            try
            {
                //get the Bp from the JWT              
                var bpId = GetBpIdFromClaims();
                var jwt = GetJWToken();
                var resp = await _moveInLogic.DeleteBprelationship(bpId.ToString(), jwt, bp);
                result = Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to DeleteCustomerContact BP for a customer", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Create Authorized Contact and create or update a bp relation ship with the parent bp in MCF.
        /// </summary>
        /// <param name="authorizedContactRequest"></param>
        /// <returns>returns CreateAuthorizedContact</returns>
        [ProducesResponseType(typeof(AuthorizedContactResponse), StatusCodes.Status200OK)]
        [HttpPost("authorized-contact")]
        public async Task<IActionResult> CreateAuthorizedContact([FromBody] AuthorizedContactRequest authorizedContactRequest)
        {
            IActionResult result;
            _logger.LogInformation($"CreateAuthorizedContact({nameof(authorizedContactRequest)}: {authorizedContactRequest.ToJson()})");

            try
            {
                if (authorizedContactRequest == null || authorizedContactRequest.AuthorizedContact == null)
                {
                    return BadRequest();
                }
                var bpId = GetBpIdFromClaims();
                var jwt = GetJWToken();
                var contact = await _moveInLogic.CreateAuthorizedContact(authorizedContactRequest, bpId.ToString(), jwt);

                result = Ok(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable Create Authorized Contact", ex.Message);

                result = ex.ToActionResult();
            }

            return result;
        }

        #region Business Partner ID Type

        /// <summary>
        /// Gets all ID types but not values for a BP from MCF.
        /// </summary>
        /// <param name="tenantBpId">A tenant's business partner identification number, provided by a landlord.</param>
        /// <returns>returns IndentifierResponse</returns>
        [ProducesResponseType(typeof(GetBpIdTypeResponse), 200)]
        [HttpGet("bp-id-types")]
        public async Task<IActionResult> GetAllIdTypes([FromQuery] long? tenantBpId)
        {
            IActionResult result;
            _logger.LogInformation("GetAllIdTypes()");

            try
            {
                // set the bpId based on whether the tenant's bpId has been provided by landlord
                var bpId = tenantBpId ?? GetBpIdFromClaims();

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
        /// Get ID type but not value for a BP from MCF.
        /// </summary>
        /// <param name="type">Represents identifier types such as last 4 SSN, drivers license number, etc.</param>
        /// <returns>returns IndentifierResponse</returns>
        [ProducesResponseType(typeof(GetBpIdTypeResponse), 200)]
        [HttpGet("bp-id-type/{type}")]
        public async Task<IActionResult> GetIdType([FromRoute] IdentifierType type, [FromQuery] long? tenantBpId)
        {
            IActionResult result;
            _logger.LogInformation($"GetIdType({nameof(type)}: {type.ToJson()})");

            try
            {
                // set the bpId based on whether the tenant's bpId has been provided by landlord
                result = base.Ok(new GetBpIdTypeResponse
                {
                    Identifiers = await _moveInLogic.GetIdType(tenantBpId ?? GetBpIdFromClaims(), type)
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
        /// Create identifier for a BP in MCF.
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
        /// Update identifier for a BP in MCF.
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
        /// <returns>returns ValidateIdTypeResponse with true or false</returns>
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
                    PiiMatch = await _moveInLogic.ValidateIdType(identifierRequest)
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

        /// <summary>
        /// retruns the related bp ids and their relationship category
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        private BpRelationResponse MapToBpRelation(BpRelationshipsResponse resp)
        {
            List<Bprelationship> relationships = new List<Bprelationship>();
            foreach (var rel in resp?.RelationShips)
            {
                var relResp = new Bprelationship()
                {
                    PrimaryBpId = rel.AuthorizedBpId,
                    AuthorizedBpId = rel.AuthorizedBpId,
                    Relationshipcategory = rel.Relationshipcategory,

                };
                relationships.Add(relResp);
            }

            var response = new BpRelationResponse()
            {
                BpRelationShips = relationships
            };
            return response;
        }

        #endregion
    }
}