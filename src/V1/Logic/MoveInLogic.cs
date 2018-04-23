using Microsoft.Extensions.Logging;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Logic.Extensions;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Logic
{
    /// <inheritdoc />
    public class MoveInLogic : IMoveInLogic
    {
        private readonly ILogger<MoveInLogic> _logger;
        private readonly IMcfClient _mcfClient;
        private static string coreLanguage = "EN";
        private readonly IAddressApi _addressApi;
        private readonly IRequestContextAdapter _requestContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveInLogic"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mcfClient">The mcfClient..</param>
        /// <param name="addressApi">The addressApi.</param>
        /// <param name="requestContext">The request context adapter.</param>
        public MoveInLogic(ILogger<MoveInLogic> logger, IMcfClient mcfClient, IAddressApi addressApi, IRequestContextAdapter requestContext)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mcfClient = mcfClient ?? throw new ArgumentNullException(nameof(mcfClient));
            this._addressApi = addressApi ?? throw new ArgumentNullException(nameof(addressApi));
            this._requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
        }

        /// <inheritdoc />
        public ReconnectStatusResponse GetMoveInLatePayment(long contractAccountId, string jwt)
        {
            _logger.LogInformation($"Getting elibigbility info: GetMoveInLatePaymentResponse({nameof(contractAccountId)} : {contractAccountId})");
            var paymentResponse = _mcfClient.GetMoveInLatePaymentsResponse(contractAccountId, jwt);

            var reconnectStatus = new ReconnectStatusResponse()
            {
                IsEligibile = paymentResponse.EligibleRc ?? false,
                AmountPosted = paymentResponse.IncPayment,
                MinimumPaymentRequired = paymentResponse.MinPayment,
                AmountLeftover = paymentResponse.MinPayment - paymentResponse.IncPayment
            };

            return reconnectStatus;
        }

        //public ReconnectionResponse GetReconnectAmountDueAndCa(string contractAccountId)
        //{
        //    var paymentResponse = _mcfClient.GetMoveInLatePaymentsResponse(contractAccountId);
        //}

        /// <inheritdoc />
        /// <summary>
        /// Checks for and returns business partner if an existing match is found.
        /// </summary>
        /// <param name="request">A business partner search request.</param>
        /// <returns>A business partner search response</returns>
        public BpSearchModel GetDuplicateBusinessPartnerIfExists(BpSearchRequest request)
        {
            try
            {
                this._logger.LogInformation($"GetDuplicateBusinessPartnerIfExists({nameof(request)}: {request})");
                var mcfResponse = this._mcfClient.GetDuplicateBusinessPartnerIfExists(request, this._requestContext.RequestChannel);

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
                throw ex;
            }

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

        #region private
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

        #endregion

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
        public Task<List<IdentifierModel>> GetAllIdTypes(long bpId)
        {
            var usersIdentifierModels = new List<IdentifierModel>();

            var response = _mcfClient.GetAllIdentifiers(bpId.ToString());
            if (response?.Result != null)
            {
                usersIdentifierModels = response.Result.ToModel();
            }

            return Task.FromResult(usersIdentifierModels);
        }

        /// <inheritdoc />
        public Task<List<IdentifierModel>> GetIdType(long bpId, IdentifierType type)
        {
            var usersIdentifierModels = new List<IdentifierModel>();

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
        public Task<bool> ValidateIdType(IdentifierRequest identifierRequest)
        {
            var response = _mcfClient.GetAllIdentifiers(identifierRequest.BpId);
            var validIdentifier = response?.Result?.Results?.FirstOrDefault(x =>
                x.IdentifierType == identifierRequest.IdentifierType.ToString() &&
                x.IdentifierNo == identifierRequest.IdentifierNo);

            return Task.FromResult(validIdentifier != null);
        }
    }
}