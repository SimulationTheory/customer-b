using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Account.Models.Request;
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
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Exceptions.Types;

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
        private readonly IAccountApi _accountApi;
        private readonly ICustomerRepository _customerRespository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveInLogic"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mcfClient">The mcfClient.</param>
        /// <param name="addressApi">The addressApi.</param>
        /// <param name="accountApi"></param>
        /// <param name="deviceApi">The deviceApi.</param>
        /// <param name="customerLogic"></param>
        public MoveInLogic(ILogger<MoveInLogic> logger, IMcfClient mcfClient, IAddressApi addressApi, IAccountApi accountApi, IDeviceApi deviceApi, ICustomerLogic customerLogic, ICustomerRepository customerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mcfClient = mcfClient ?? throw new ArgumentNullException(nameof(mcfClient));
            _addressApi = addressApi ?? throw new ArgumentNullException(nameof(addressApi));
            _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
            _deviceApi = deviceApi ?? throw new ArgumentNullException(nameof(deviceApi));
            _customerLogic = customerLogic ?? throw new ArgumentNullException(nameof(customerLogic));
            _customerRespository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        /// <inheritdoc />
        public ReconnectStatusResponse GetMoveInLatePayment(long contractAccountId, bool reconnectionFlag, string jwt)
        {
            _logger.LogInformation($"Getting elibigbility info: GetMoveInLatePaymentResponse({nameof(contractAccountId)} : {contractAccountId})");
            var paymentResponse = _mcfClient.GetMoveInLatePaymentsResponse(contractAccountId, reconnectionFlag, jwt);

            var reconnectStatus = new ReconnectStatusResponse()
            {
                IsEligibile = paymentResponse.EligibleRc ?? false,
                Reconnect = paymentResponse.ReconnectFlag ?? false,
                ContractAccountId = paymentResponse.AccountNo,
                PriorObligationContractAccountId = paymentResponse.PriorObligationAccount,
                AmountPosted = paymentResponse.IncPayment,
                MinimumPaymentRequired = paymentResponse.MinPayment,
                AmountLeftover = paymentResponse.MinPayment - paymentResponse.IncPayment,
                Deposit = paymentResponse.DepositAmount,
                ReconnectAmount = paymentResponse.ReconAmount,
                Reason = paymentResponse.Reason,
                ReasonCode = paymentResponse.ReasonCode,
                AccountType = paymentResponse.AccountType,
                FirstLp = paymentResponse.FirstIp,
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

                    if (mcfResponse.Threshhold != null
                                            && mcfResponse.Threshhold.ToUpper().Contains("X")
                                            && mcfResponse.Unique != null
                                            && Convert.ToInt32(mcfResponse.Unique) == 1)
                    {
                        response.MatchFound = true;
                        response.BpId = Convert.ToInt64(mcfResponse.BpId);
                        response.BpSearchIdentifiers = mcfResponse.BpSearchIdInfoSet.Results?.ToList();
                        response.Reason = mcfResponse.Reason;
                        response.ReasonCode = mcfResponse.ReasonCode;
                    }
                    else
                    {
                        response.MatchFound = false;
                        var resultCount = mcfResponse.BpSearchIdInfoSet != null ? mcfResponse.BpSearchIdInfoSet.Results.ToList().Count : 0;
                        response.Reason = string.IsNullOrEmpty(mcfResponse.Reason) ? $"{resultCount} search results found as partial match." : mcfResponse.Reason;
                        response.ReasonCode = string.IsNullOrEmpty(mcfResponse.ReasonCode) ? $"Threshhold not met." : mcfResponse.ReasonCode;
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
                return new CancelMoveInResponse()
                {
                    Success = cancelResponse.Success,
                    StatusMessage = cancelResponse.StatusMessage
                };
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
                           
                if (resp.BpId != "0")
                {
                    UpdateCustomerDetailsInCassandra(request, Int64.Parse(resp.BpId), false);
                }
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
        public async Task<BpRelationshipsResponse> GetBprelationships(BpRelationshipRequestParam bprelationRequestParam)
        {
            _logger.LogInformation($"GetBprelationshipsr: GetBprelationships({nameof(bprelationRequestParam.LoggedInBp)} : {bprelationRequestParam.LoggedInBp})");
            try
            {
                BpRelationshipsMcfResponse resp;
                string bpToMap;
                if (!string.IsNullOrEmpty(bprelationRequestParam.TenantBp))
                {
                    resp = await _mcfClient.GetBprelationships(bprelationRequestParam.TenantBp);
                    bpToMap = bprelationRequestParam.TenantBp;
                }
                else
                {
                    resp = await _mcfClient.GetBprelationships(bprelationRequestParam.LoggedInBp, bprelationRequestParam.Jwt);
                    bpToMap = bprelationRequestParam.LoggedInBp;
                }
                
                var bprelations = MapBpRelations(resp, bprelationRequestParam.LoggedInBp);

                return bprelations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to GetBprelationships for {bprelationRequestParam.LoggedInBp}");
                throw ex;
            }
        }

        /// <summary>
        /// Delete  relationships given a business partner id
        /// </summary>
        /// <param name="bpId"></param>
        ///  <param name="jwt"></param>
        ///  <param name="bpTodelete"></param>
        /// <returns></returns>
        public async Task<BpRelationshipUpdateResponse> DeleteBprelationship(string bpId, string jwt, string bpTodelete)
        {
            _logger.LogInformation($"DeleteBprelationship: DeleteBprelationship({nameof(bpId)} : {bpId})");
            try
            {
                var resp = await _mcfClient.GetBprelationships(bpId, jwt);
                var bprelations = MapBpRelations(resp, bpId);
                var hasRelation = CheckRelationWithContact(bprelations, bpTodelete);

                var hasActiveRelation = IsrelationShipActive(hasRelation);

                if (hasRelation == null || !hasActiveRelation)
                {
                    throw new BadRequestException(
                       $"The relationship between {bpId} and {bpTodelete} doesn't exist or is inactive");

                }
                //Update
                var relationShipToupdate = GetRelationshipToDelete(hasRelation);
                var updateResponse = _mcfClient.UpdateBusinessPartnerRelationship(relationShipToupdate, jwt);

                return updateResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to DeleteBprelationship for {bpId}");
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
                    var bpRelationParam = new BpRelationshipRequestParam()
                    {
                        LoggedInBp = loggedInBp,
                        Jwt = jwt,
                        TenantBp = null //TODO add TenanID

                    };
                    var checkRelationShip = await GetBprelationships(bpRelationParam);
                    var hasRelation = CheckRelationWithContact(checkRelationShip, contactBp);
                    var hasActiveRelation = IsrelationShipActive(hasRelation);

                    if (hasRelation != null && !hasActiveRelation)
                    {
                        //Update
                        var relationShipToupdate = GetRelationshipToupdate(hasRelation);
                        _mcfClient.UpdateBusinessPartnerRelationship(relationShipToupdate, jwt);

                    }
                    if (hasRelation == null)
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
                ContractItemNav = await CreateContractItemNavList(request),
                ProdAttributes = new List<ProdAttribute>()
            };

            var response = _mcfClient.PostMoveIn(mcfMoveInRequest, jwt);

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

        /// <inheritdoc />
        public async Task<CleanMoveInResponse> PostCleanMoveIn(CleanMoveInRequest request, string jwt)
        {

            var bpId = ParseBpFromString(request.tenantBPId);

            //
            var addressInfoResponse = await _customerLogic.GetMailingAddressesAsync(bpId, true, jwt);
            var addressInfo = addressInfoResponse.FirstOrDefault();

            var accountRequest = new CreateAccountRequest()
            {
                Description = "New Move In"
            };
            var accountResponse = await _accountApi.PostCreateContractAccount(accountRequest);

            var mcfMoveInRequest = new CreateMoveInRequest()
            {
                AccountID = request.tenantBPId,
                CustomerRole = "",
                ProcessType = "",
                ContractItemNav = await CreateCleanMoveInContractItemNavList(request, accountResponse.ContractAccountId.ToString()),
                ProdAttributes = CreateProdAttributeList(request.ProductEnrollments)
            };

            var response = _mcfClient.PostMoveIn(mcfMoveInRequest, jwt);
            var depositByContractId = response.ContractItemNav.Results.ToDictionary(contractItem => contractItem.ContractID, contractItem => contractItem.SecDepositAmt);
            var notificationNumberByContractId = response.ContractItemNav.Results.ToDictionary(
                contractItem => contractItem.ContractID, contractItem => contractItem.NotificationNumber);
            var moveInResponse = new CleanMoveInResponse()
            {
                DepositsByContractId = depositByContractId,
                NotificationNumberByContractId = notificationNumberByContractId
            };
            return moveInResponse;
        }

        #region Helper Methods

        private async Task<IEnumerable<ContractItemNav>> CreateContractItemNavList(MoveInRequest request)
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

        public async Task<IEnumerable<ContractItemNav>> CreateCleanMoveInContractItemNavList(CleanMoveInRequest request, string newContractAccountId)
        {
            var contractItemNavList = new List<ContractItemNav>();
            foreach (var installation in request.Installations)
            {
                var contractItemNav = new ContractItemNav()
                {
                    ContractStartDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ContractEndDate = DateTime.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
                    BusinessAgreementID = newContractAccountId,
                    TransferCA = "",
                    ProductID = GetProductId(installation.DivisionId),
                    DivisionID = installation.DivisionId,
                    PointOfDeliveryGUID = installation.InstallationGuid
                };

                contractItemNavList.Add(contractItemNav);
            }

            return contractItemNavList;
        }

        //TODO: Speak with Venu to revisit this 4/30/2018
        public IEnumerable<ProdAttribute> CreateProdAttributeList(IEnumerable<ProductInfo> products)
        {
            var prodAttributes = new List<ProdAttribute>();

            if (products == null) return prodAttributes;

            //TODO: Find other product type names

            foreach (var product in products)
            {
                var productType = product.ProductType.ToLower();
                var prodAttribute = new ProdAttribute();
                switch (productType)
                {
                    case "green":
                        prodAttribute.Attrname = "ZFC_GPWBLK";
                        break;
                    case "carbon":
                        prodAttribute.Attrname = "ZFC_CARBOFF";
                        break;
                    default:
                        throw new BadRequestException("Unknown product type. Please verify product type is correct.");
                }
                prodAttribute.Attrvalue = product.EnrollmentAmount.ToString();
                prodAttributes.Add(prodAttribute);

            }

            return prodAttributes;

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
            foreach (var rel in bprelationships)
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
                Differentiationtypevalue = "",
                Relationshiptypenew = "",
                Validfromdate = relationShip.Validfromdate,
                Validtodate = relationShip.Validtodate,
                //Validfromdatenew = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                //Validtodatenew = DateTimeOffset.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
                Validfromdatenew = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                Validtodatenew = DateTimeOffset.MaxValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"),
                Relationshipcategory = relationShip.Relationshipcategory,
            };
            return updateRelation;
        }

        private BpRelationshipUpdateRequest GetRelationshipToDelete(BpRelationshipResponse relationShip)
        {
            var updateRelation = new BpRelationshipUpdateRequest()
            {
                AccountID1 = long.TryParse(relationShip.BpId1, out long bp1) ? bp1 : 0,
                AccountID2 = long.TryParse(relationShip.BpId2, out long bp2) ? bp2 : 0,
                Defaultrelationship = false,
                Differentiationtypevalue = "",
                Relationshiptypenew = "",
                Validfromdate = relationShip.Validfromdate,
                Validtodate = relationShip.Validtodate,
                Validfromdatenew = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                Validtodatenew = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
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
                //Validfromdate = relationShip.Validfromdate,
                //Validtodate = relationShip.Validtodate,
                Validfromdatenew = DateTimeOffset.Parse(fromDate),
                Validtodatenew = DateTimeOffset.Parse(toDate),
            };



            return mcfRequest;
        }

        private bool IsrelationShipActive(BpRelationshipResponse checkRelationShip)
        {
            bool valid = true;

            if (checkRelationShip?.Validtodate == null)
            {
                throw new ArgumentException("Invalid Date");
            }

            if (checkRelationShip?.Validfromdate == null)
            {
                throw new ArgumentException("Invalid Date");
            }
            var to = "\"" + checkRelationShip?.Validtodate + "\"";
            var from = "\"" + checkRelationShip?.Validfromdate + "\"";

            DateTimeOffset toDate = JsonConvert.DeserializeObject<DateTimeOffset>(to);
            DateTimeOffset fromDate = JsonConvert.DeserializeObject<DateTimeOffset>(from);


            if (toDate < DateTime.UtcNow || fromDate > DateTime.UtcNow)
            {
                valid = false;
            }
            return valid;
        }

        private BpRelationshipResponse CheckRelationWithContact(BpRelationshipsResponse checkRelationShip, string contactBp)
        {
            var bprelation = checkRelationShip?.RelationShips?.FirstOrDefault(r => (r.BpId1 == contactBp || r.BpId2 == contactBp));
            return bprelation;
        }
        private long ParseBpFromString(string bp)
        {
            if (!long.TryParse(bp, out var bpId))
            {
                throw new InternalServerException($"{bp} should be Long data type");
            }

            return bpId;
        }
        private bool UpdateCustomerDetailsInCassandra(CreateBusinesspartnerRequest createBusinessPartnerData, long bpID, bool update)
        {
            try
            {
                _customerRespository.UpdateCassandraCustomerInformation(bpID, createBusinessPartnerData);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cassandra update was not successful for bp id :{bpID.ToString()} {ex.Message}");
                return false;
            }
        }

        #endregion

    }
}
     