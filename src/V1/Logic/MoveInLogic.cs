using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Device.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Logic.Extensions;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions.Types;
using PSE.WebAPI.Core.Service.Interfaces;

namespace PSE.Customer.V1.Logic
{
    /// <inheritdoc />
    public class MoveInLogic : IMoveInLogic
    {
        private readonly ILogger<MoveInLogic> _logger;
        private readonly IMcfClient _mcfClient;
        private static string coreLanguage = "EN";
        private readonly IAddressApi _addressApi;
        private readonly ICustomerLogic _customerLogic;
        private const string McfDateFormat = "yyyy-MM-ddThh:mm:ss";
        private static string ValidToMcfmaxdata = new DateTime(9999, 12, 31).ToString(McfDateFormat);
        private readonly IDeviceApi _deviceApi;

        public MoveInLogic(ILogger<MoveInLogic> logger, IMcfClient mcfClient, IAddressApi addressApi, IDeviceApi deviceApi,ICustomerLogic customerLogic)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mcfClient = mcfClient ?? throw new ArgumentNullException(nameof(mcfClient));
            _addressApi = addressApi ?? throw new ArgumentNullException(nameof(addressApi));
            _deviceApi = deviceApi ?? throw new ArgumentNullException(nameof(deviceApi));
            _customerLogic = customerLogic ?? throw new ArgumentNullException(nameof(customerLogic));
        }

        /// <inheritdoc />
        public ReconnectStatusResponse GetMoveInLatePayment(long contractAccountId, bool reconnectionFlag, string jwt)
        {
            _logger.LogInformation($"Getting elibigbility info: GetMoveInLatePaymentResponse({nameof(contractAccountId)} : {contractAccountId})");
            var paymentResponse = _mcfClient.GetMoveInLatePaymentsResponse(contractAccountId, reconnectionFlag, jwt);

            var reconnectStatus = new ReconnectStatusResponse()
            {
                IsEligibile = paymentResponse.EligibleRc ?? false,
                AmountPosted = paymentResponse.IncPayment,
                MinimumPaymentRequired = paymentResponse.MinPayment,
                AmountLeftover = paymentResponse.MinPayment - paymentResponse.IncPayment
            };

            return reconnectStatus;
        }


        /// <inheritdoc />
        public async Task<BpSearchModel> GetDuplicateBusinessPartnerIfExists(BpSearchRequest request)
        {
            try
            {
                _logger.LogInformation($"GetDuplicateBusinessPartnerIfExists({nameof(request)}: {request})");
                var mcfResponse = await _mcfClient.GetDuplicateBusinessPartnerIfExists(request);

                // if these Threshhold and Unique conditions are met
                // we can return the response and bpid
                //      unique = 1 (good bp to use)
                //      threshold = x(true..has been met)
                if (mcfResponse != null)
                {
                    var response = new BpSearchModel();

                    if (mcfResponse.Threshhold.ToUpper().Contains("X") && Convert.ToInt32(mcfResponse.Unique) == 1)
                    {
                        response.MatchFound = true;
                        response.BpId = Convert.ToInt64(mcfResponse.BpId);
                        response.BpSearchIdentifiers = mcfResponse.BpSearchIdInfoSet.Results.ToList();
                        response.Reason = mcfResponse.Reason;
                        response.ReasonCode = mcfResponse.ReasonCode;
                    }
                    else
                    {
                        response.MatchFound = false;
                        response.Reason = string.IsNullOrEmpty(mcfResponse.Reason) ? "The threshhold for a match was not met." : mcfResponse.Reason;
                        var resultCount = mcfResponse.BpSearchIdInfoSet.Results.ToList().Count;
                        response.ReasonCode = string.IsNullOrEmpty(mcfResponse.ReasonCode) ? $"{resultCount} records returned as a possible match." : mcfResponse.Reason;
                    }

                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Failed to search for Business Partner.");
                throw e;
            }
        }

        /// <inheritdoc />
        public async Task<CancelMoveInResponse> PostCancelMoveIn(CancelMoveInRequest request)
        {
            _logger.LogInformation($"Cancelling Move In: CancelMoveInForContractId({nameof(request)} : {request.ToJson()})");

            try
            {
                var cancelResponse = await _mcfClient.PostCancelMoveIn(request);
                return cancelResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to cancel move in.");
                throw e;
            }
        }

        /// <summary>
        /// Calls Addres APi to get MCf address and then calls MCF to create Business partner
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateBusinesspartnerResponse> CreateBusinessPartner(CreateBusinesspartnerRequest request)
        {
            _logger.LogInformation($"Creating Business Partner: CreateBusinessPartner({nameof(request)} : {request})");

            try
            {
                var address = request.Address;
                var addressResponse = await _addressApi.ToMcfMailingAddressAsync(address);
                var createBpresponse = ValidateAddress(addressResponse);

                if (createBpresponse != null)
                {
                    return createBpresponse;
                }

                var addressInfo = addressResponse?.Data;
                var businessPartnerMcfrequest = GetBusinessPartnerMcfRequest(request, addressInfo);
                var mcfResp = await _mcfClient.CreateBusinessPartner(businessPartnerMcfrequest);
                var resp = new CreateBusinesspartnerResponse()
                {
                    BpId = mcfResp.PartnerId
                };

                return resp;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Buiness Partner");
                throw;
            }

        }


        /// <summary>
        /// Get Bp relationship based on a given BP
        /// </summary>
        /// <param name="bpId"></param>
        /// /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task<BpRelationshipsResponse> GetBprelationships(string bpId, string jwt)
        {
            _logger.LogInformation($"GetBprelationshipsr: GetBprelationships({nameof(bpId)} : {bpId})");
            try
            {
                var resp = await _mcfClient.GetBprelationships(bpId, jwt);
                var bprelations = MapBpRelations(resp, bpId);
              
                return bprelations;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to GetBprelationships for {bpId}");
                throw ex;
            }
            
        }
        /// <summary>
        /// Creates Autorized contact
        /// Get Customer Relationships Get /v{version}/customer/bp-relationships
         ///Check to see if customer is existing(and active) contact customer 
         ///   If yes and active – no action required
         ///   If yes and not currently valid – Update valid to date to 1231999? 
        ///       If no – create relationship type contact customer between the two BP;s
         ///   From Date: System Date
         ///   To Date: 12/29/9999 
        /// </summary>
        /// <param name="authorizedContactRequest"></param>
        /// <param name="bpId"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task<AuthorizedContactResponse> CreateAuthorizedContact(AuthorizedContactRequest authorizedContactRequest, string bpId, string jwt)
        {
            _logger.LogInformation($"CreateAuthorizedContact: CreateAuthorizedContact({nameof(authorizedContactRequest)} : {authorizedContactRequest.ToJson()})");
            
            try
            {
                var authorizedContactresponse = new AuthorizedContactResponse();
                //Do a bp search
                var bpSearch = MapToBpSearchrequest(authorizedContactRequest.AuthorizedContact);
                var bpExists = await GetDuplicateBusinessPartnerIfExists(bpSearch);
                var loggedInBp = bpId.ToString();
                if (bpExists == null || !bpExists.MatchFound)
                {
                    //if bp doesn't exist
                    // create bp
                    //Create a relation ship
                    _logger.LogInformation($"There is not a match for an existing Business Partner based on the information provided.");
                    var bp = await CreateBusinessPartner(authorizedContactRequest.AuthorizedContact); // create bp
                    authorizedContactresponse.BpId = bp.BpId;
                    //Create a Bp relation ship
                    var bpCreateRelation = new CreateBpRelationshipRequest()
                    {
                        FirstAccountBpId = bpId,
                        SecondAccountBpId = bp.BpId,
                        Relationshipcategory = authorizedContactRequest.AuthorizedContact?.Relationshipcategory.GetEnumMemberValue()
                    };
                    var createRelation = _customerLogic.CreateBpRelationshipAsync(bpCreateRelation, jwt);

                }
                else
                {
                    //if bp exist 
                    // check if Bp relation ship exist , if yes update relationship
                    // if no relation ship exist then create a relation ship
                    var contactBp = bpExists.BpId.ToString();
                    authorizedContactresponse.BpId = bpExists.BpId.ToString();
                    var checkRelationShip = await GetBprelationships(loggedInBp, jwt);
                    var hasRelation = CheckRelationWithContact(checkRelationShip, contactBp);
                    var hasActiveRelation = IsrelationShipActive(hasRelation);

                    if (hasRelation != null && !hasActiveRelation)
                    {
                        //Update
                        var relationShipToupdate = GetRelationshipToupdate(hasRelation);
                        _mcfClient.UpdateBusinessPartnerRelationship(relationShipToupdate, jwt);
                      
                    }
                    if(hasRelation == null)
                    {
                        //Create Bp relationship
                        var bpCreateRelation = new CreateBpRelationshipRequest()
                        {
                            FirstAccountBpId = bpId,
                            SecondAccountBpId = contactBp,
                            Relationshipcategory = authorizedContactRequest.AuthorizedContact?.Relationshipcategory.GetEnumMemberValue()
                        };
                        var createRelation = _customerLogic.CreateBpRelationshipAsync(bpCreateRelation, jwt);
                    }

                }
                return authorizedContactresponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to CreateAuthorizedContact");
                throw ex;
            }
            
        }

        /// <inheritdoc/>
        public List<DateTimeOffset> GetInvalidMoveinDates(GetInvalidMoveinDatesRequest invalidMoveinDatesRequest)
        {
            List<DateTimeOffset> dates = null;

            var mcfResponse = _mcfClient.GetInvalidMoveinDates(invalidMoveinDatesRequest);

            if (mcfResponse != null)
            {
                // Note verified with Vikas/Bimba that HolidaysResultList will always return a list with one element.
                dates = new List<DateTimeOffset>();
                foreach (Holiday holiday in mcfResponse.Result.HolidaysResultList[0].HolidaysNav.Holidays)
                {
                    dates.Add(holiday.Date);
                }
            }

            return dates;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<long>> PostPriorMoveIn(MoveInRequest request, long bp, string jwt)
        {
            var mcfMoveInRequest = new CreateMoveInRequest()
            {
                AccountID = bp.ToString(),
                CustomerRole = "",
                ProcessType = "PRIOR",
                ContractItemNav = await CreateContractItemNavList(request, bp, jwt),
                ProdAttributes = new List<ProdAttributes>()
            };

            var response = _mcfClient.PostPriorMoveIn(mcfMoveInRequest, jwt);

            var newContractAccounts = response.ContractItemNav.Results.Select(item => long.Parse(item.BusinessAgreementID)).ToList();

            return newContractAccounts;

        }

        /// <inheritdoc />
        public Task<List<IdentifierTypeResponse>> GetAllIdTypes(long bpId)
        {
            var usersIdentifierModels = new List<IdentifierTypeResponse>();

            var response = _mcfClient.GetAllIdentifiers(bpId.ToString());
            if (response?.Result != null)
            {
                usersIdentifierModels = response.Result.ToModel();
            }

            return Task.FromResult(usersIdentifierModels);
        }

        /// <inheritdoc />
        public Task<List<IdentifierTypeResponse>> GetIdType(long bpId, IdentifierType type)
        {
            var usersIdentifierModels = new List<IdentifierTypeResponse>();

            var response = _mcfClient.GetAllIdentifiers(bpId.ToString());
            if (response?.Result?.Results != null)
            {
                var validIds = response.Result.ToModel();
                var matchingType = validIds.FirstOrDefault(x => x.IdentifierType == type);
                if (matchingType != null)
                {
                    usersIdentifierModels.Add(matchingType);
                }
            }

            return Task.FromResult(usersIdentifierModels);
        }

        /// <inheritdoc />
        public Task<CreateBpIdTypeResponse> CreateIdType(IdentifierRequest identifierRequest)
        {
            var bpIdentifier = new BpIdentifier(identifierRequest);
            var response = new CreateBpIdTypeResponse();

            var mcfResponse = _mcfClient.CreateIdentifier(bpIdentifier);
            if (mcfResponse == null)
            {
                const string uiFailureMessage = "Failed to create business partner identifier";
                _logger.LogError($"{ uiFailureMessage }: {identifierRequest.ToJson()}");
                throw new InternalServerException(uiFailureMessage);
            }

            response.HttpStatusCode = mcfResponse.HttpStatusCode;
            if (mcfResponse.Error != null)
            {
                const string uiFailureMessage = "Failed to create business partner identifier";
                _logger.LogError($"{ mcfResponse.Error.ToJson() }: {identifierRequest.ToJson()}");
                response.ErrorMessage = uiFailureMessage;
            }
            else
            {
                response.BpIdType = mcfResponse.Result.ToModel();
            }

            return Task.FromResult(response);
        }

        /// <inheritdoc />
        public Task<StatusCodeResponse> UpdateIdType(IdentifierRequest identifierRequest)
        {
            var bpIdentifier = new BpIdentifier(identifierRequest);

            var response = _mcfClient.UpdateIdentifier(bpIdentifier);
            if (response == null || response.Error != null)
            {
                const string uiFailureMessage = "Failed to update business partner identifier";
                var responseErrorMessage = response?.Error != null ? response.Error.ToJson() : uiFailureMessage;
                _logger.LogError($"{ responseErrorMessage }: {identifierRequest.ToJson()}");
                throw new InternalServerException(uiFailureMessage);
            }

            return Task.FromResult(response.ToModel());
        }

        /// <inheritdoc />
        public Task<bool> ValidateIdType(IdentifierRequest identifierRequest)
        {
            var response = _mcfClient.GetAllIdentifiers(identifierRequest.BpId);
            if (response == null || response.Error != null)
            {
                const string uiFailureMessage = "Failed to get business partner identifiers";
                var responseErrorMessage = response?.Error != null ? response.Error.ToJson() : uiFailureMessage;
                _logger.LogError($"{ responseErrorMessage }: {identifierRequest.ToJson()}");
                throw new InternalServerException(uiFailureMessage);
            }

            var validIdentifier = response.Result?.Results?.FirstOrDefault(x =>
                x.IdentifierType == identifierRequest.IdentifierType.ToString() &&
                x.IdentifierNo == identifierRequest.IdentifierNo);

            return Task.FromResult(validIdentifier != null);
        }

        #region Helper Methods

        private async Task<IEnumerable<ContractItemNav>> CreateContractItemNavList(MoveInRequest request, long bp, string jwt)
        {
            var premiseInstallation = await _deviceApi.GetPremiseInstallation(request.PremiseId);
            var contractItemNavList = new List<ContractItemNav>();
            foreach (var installationId in request.InstallationIds)
            {
                var premiseInstallDetails = premiseInstallation.Data.Installations.FirstOrDefault(x => x.InstallationId == installationId);
                if (premiseInstallDetails == null)
                    throw new BadRequestException(
                        "Unable to find premise installation details with provided installation id");
                var contractItemNav = new ContractItemNav()
                {
                    ContractStartDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ContractEndDate = DateTime.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
                    BusinessAgreementID = request.ContractAccountId.ToString(),
                    TransferCA = "",
                    ProductID = GetProductId(premiseInstallDetails.DivisionId),
                    DivisionID = premiseInstallDetails.DivisionId,
                    PointOfDeliveryGUID = premiseInstallDetails.InstallationGuid,

                };

                contractItemNavList.Add(contractItemNav);
            }

            return contractItemNavList;
        }
        public string GetProductId(string divisionId)
        {
            var electricProductId = "DEF_ELE";
            var gasProductId = "DEF_GAS";
            switch (divisionId)
            {
                case "10":
                    return electricProductId;
                case "20":
                    return gasProductId;
                default:
                    return "";
            }

        }

        private CreateBusinesspartnerMcfRequest GetBusinessPartnerMcfRequest(CreateBusinesspartnerRequest request, McfAddressinfo addressInfo)
        {
            var businessMcfRequest = new CreateBusinesspartnerMcfRequest()
            {
                Channel = request.Channel,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                EMail = request.Email,
                OrgName = request.OrgName,
                CorrLanguage = coreLanguage,
                PartnerCategory = GetPartnerCategory(request.PartnerCategory),
                PartnerRole = GetPartnerRole(request.PartnerCategory),
                Phone = request?.Phone?.Number,
                PhoneType = GetPhoneType(request?.Phone),
                Extension = request?.Phone?.Extension,
                AddressType = GetAddressTye(addressInfo.AddressType),
                CareOf = addressInfo.COName,
                Street = addressInfo.Street,
                HouseNum = addressInfo.HouseNo,
                City = addressInfo.City,
                PostalCode = addressInfo.PostalCode,
                PoBox = addressInfo.POBox,
                State = addressInfo.Region,
                Country = addressInfo.CountryID
            };

            return businessMcfRequest;
        }

        private string GetAddressTye(McfAddressType addressType)
        {
            string type = string.Empty;
            switch (addressType)
            {
                case McfAddressType.USStandard:
                    type = "1";
                    break;
                case McfAddressType.POBoxAndPMB:
                    type = "2";
                    break;
                case McfAddressType.Foreign:
                    type = "3";
                    break;
                case McfAddressType.Military:
                    type = "4";
                    break;
                case McfAddressType.General:
                    type = "5";
                    break;
            }
            return type;

        }

        private string GetPartnerRole(PartnerCategoryType partnerCategory)
        {
            string partnerRole = string.Empty;
            switch (partnerCategory)
            {
                case PartnerCategoryType.Residential:
                case PartnerCategoryType.Organization:
                    partnerRole = "CRM000";
                    break;

                case PartnerCategoryType.AuthorizedContact:
                    partnerRole = "BUP001";
                    break;
            }

            return partnerRole;
        }

        private string GetPhoneType(Phone phone)
        {
            string phonetype = string.Empty;
            if (phone == null) return phonetype;

            var type = phone.Type;
            switch (type)
            {
                case PhoneType.Cell:
                    phonetype = "3";
                    break;
                case PhoneType.Work:
                    phonetype = "2";
                    break;
                case PhoneType.Home:
                    phonetype = "1";
                    break;
            }
            return phonetype;

        }

        private string GetPartnerCategory(PartnerCategoryType partnerCategory)
        {
            string category = string.Empty;
            switch (partnerCategory)
            {
                case PartnerCategoryType.Residential:
                    category = "1";
                    break;
                case PartnerCategoryType.Organization:
                    category = "2";
                    break;
                case PartnerCategoryType.AuthorizedContact:
                    category = "1";
                    break;
            }

            return category;
        }

        private CreateBusinesspartnerResponse ValidateAddress(RestSharp.IRestResponse<McfAddressinfo> addressResponse)
        {
            CreateBusinesspartnerResponse response = null;
            if (!addressResponse.IsSuccessful && addressResponse?.Data == null)
            {
                var message = $"Address validator service failed with the error {addressResponse.Content}";
                _logger.LogError(message);
                if (addressResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    response = new CreateBusinesspartnerResponse()
                    {
                        ErrorMessage = message,
                        HttpStatusCode = System.Net.HttpStatusCode.BadRequest

                    };
                }
                else
                {
                    throw new Exception(message);
                }
            }
            return response;
        }

        private BpRelationshipsResponse MapBpRelations(BpRelationshipsMcfResponse resp, string bpId)
        {

            var relationShips = new BpRelationshipsResponse()
            {
                RelationShips = GetRelations(resp.Results)
            };
            return relationShips;
        }

        private List<BpRelationshipResponse> GetRelations(List<BpRelationship> bprelationships)
        {
            List<BpRelationshipResponse> relationships = new List<BpRelationshipResponse>();
           foreach(var rel in bprelationships)
           {
                var relResp = new BpRelationshipResponse()
                {
                    BpId1 = rel.AccountID2,
                    BpId2 = rel.AccountID1,
                    Message = rel.Message,
                    Relationshipcategory = rel.Relationshipcategory,
                    Validfromdate = rel.Validfromdate,
                    Validfromdatenew = rel.Validfromdatenew,
                    Validtodate = rel.Validtodate,
                    Validtodatenew = rel.Validtodatenew
                };
                relationships.Add(relResp);
           }

            return relationships;
        }

        private BpSearchRequest MapToBpSearchrequest(CreateBusinesspartnerRequest authorizedContactRequest)
        {
            var bpSearch = new BpSearchRequest()
            {
                Email = authorizedContactRequest.Email,
                FirstName = authorizedContactRequest.FirstName,
                MiddleName = authorizedContactRequest.MiddleName,
                LastName = authorizedContactRequest.LastName,
                ServiceZip = authorizedContactRequest.Address.PostalCode,
                Phone = authorizedContactRequest.Phone?.Number
            };
            return bpSearch;
        }

        private BpRelationshipUpdateRequest GetRelationshipToupdate(BpRelationshipResponse relationShip)
        {
            var updateRelation = new BpRelationshipUpdateRequest()
            {
                AccountID1 = long.TryParse(relationShip.BpId1, out long bp1) ? bp1 : 0,
                AccountID2 = long.TryParse(relationShip.BpId2, out long bp2) ? bp2 : 0,
                Defaultrelationship = false,
                Differentiationtypevalue="",
                Relationshiptypenew ="",
                Validfromdate = relationShip.Validfromdate.ToString(McfDateFormat),
                Validtodate = relationShip.Validtodate.ToString(McfDateFormat),
                Validfromdatenew = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                Validtodatenew = DateTimeOffset.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
                Relationshipcategory = relationShip.Relationshipcategory,
            };
            return updateRelation;
        }

        private BpRelationshipRequest GetRelationshipRequest(BpRelationshipResponse relationShip)
        {
            var fromDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            var toDate = DateTimeOffset.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
            var mcfRequest = new BpRelationshipRequest()
            {
                AccountID1 = relationShip.BpId1,
                AccountID2 = relationShip.BpId2,
                Relationshipcategory = relationShip.Relationshipcategory,
                Differentiationtypevalue = "",
                Defaultrelationship = false,
                Validfromdate = relationShip.Validfromdate,
                Validtodate = relationShip.Validtodate,
                Validfromdatenew = DateTime.Parse(fromDate),
                Validtodatenew = DateTime.Parse(toDate),
            };


           
            return mcfRequest;
        }

        private bool IsrelationShipActive(BpRelationshipResponse checkRelationShip)
        {
            bool valid = false;
            if(checkRelationShip?.Validtodate < DateTime.Now || checkRelationShip?.Validfromdate > DateTime.Now)
            {
                valid =  false;
            }
            return valid;
        }

        private BpRelationshipResponse CheckRelationWithContact(BpRelationshipsResponse checkRelationShip, string contactBp)
        {
           var bprelation =  checkRelationShip?.RelationShips?.FirstOrDefault(r => (r.BpId1 == contactBp || r.BpId2 == contactBp));
           return bprelation;
        }

       
        #endregion
    }
}