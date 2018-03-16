using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PSE.McfClient.Tests.Unit
{
    public class TestHttpClient : HttpClient
    {
        public static string EligibleAccountId => "200000044574";
        public static string NonEligibleAccountId => "200000044575";
        public static string ProblematicMarker => "##PROBLEM##";

        public static PaymentArrangement EligiblePaymentArrangement = new PaymentArrangement
        {
            Channel = "WEB",
            ContractAccountID = EligibleAccountId,
            NoOfInstallments = "6",
            InstallmentPlanType = "P15",
            Reason = ""
        };

        public static PaymentArrangement NonEligiblePaymentArrangement = new PaymentArrangement
        {
            Channel = "WEB",
            ContractAccountID = NonEligibleAccountId,
            NoOfInstallments = "0",
            InstallmentPlanType = "P15",
            Reason = "Bankruptcy"
        };

        public TestHttpClient() { }
        public TestHttpClient(HttpClientHandler httpClientHandler) { }

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Post)
            {
                var content = await request.Content.ReadAsStringAsync();
                var response = new HttpResponseMessage();
                if (content.Contains(EligibleAccountId))
                {
                    response.StatusCode = HttpStatusCode.Created;
                    McfContext.InsertContent(EligiblePaymentArrangement, response);
                }
                else if (content.Contains(NonEligibleAccountId))
                {
                    response.StatusCode = HttpStatusCode.Created;
                    McfContext.InsertContent(NonEligiblePaymentArrangement, response);
                }
                else if (content.Contains(ProblematicMarker))
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    McfContext.InsertContent("Error processing request", response);
                }
                return response;
            }
            if (request.Method == HttpMethod.Get)
            {
                var response = new HttpResponseMessage();
                if (request.RequestUri.AbsoluteUri.Contains(EligibleAccountId))
                {
                    response.StatusCode = HttpStatusCode.OK;
                    McfContext.InsertContent(EligiblePaymentArrangement, response);
                }
                else if (request.RequestUri.AbsoluteUri.Contains(NonEligibleAccountId))
                {
                    response.StatusCode = HttpStatusCode.OK;
                    McfContext.InsertContent(NonEligiblePaymentArrangement, response);
                }
                else if (request.RequestUri.AbsoluteUri.Contains(ProblematicMarker))
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    McfContext.InsertContent("Error processing request", response);
                }
                return response;
            }
            throw new NotImplementedException();
        }
    }
}
