using PSE.Customer.V1.Logic.Interfaces;
using System;
using Microsoft.Extensions.Logging;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Response;
using System.Threading.Tasks;
using PSE.Customer.V1.Request;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using System.Collections.Generic;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Request;

namespace PSE.Customer.V1.Logic
{
    public class MoveInLogic : IMoveInLogic
    {
        private readonly ILogger<MoveInLogic> _logger;
        private readonly IMcfClient _mcfClient;
        private static string coreLanguage = "EN";
        private readonly IAddressApi _addressApi;

        public MoveInLogic(ILogger<MoveInLogic> logger, IMcfClient mcfClient, IAddressApi addressApi)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mcfClient = mcfClient;
            _addressApi = addressApi ?? throw new ArgumentNullException(nameof(addressApi));
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

                if(createBpresponse != null)
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
            switch(partnerCategory)
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
    }
}
