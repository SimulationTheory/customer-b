using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSE.McfClient.Tests.Unit
{
    public class TestRestClient : RestClient
    {
        public TestRestClient(Uri uri) { }

        public override IRestResponse Execute(IRestRequest request)
        {
            if (request.Resource.Contains("mcf-token"))
            {
                var response = new RestResponse();
                response.Content = "{\"mcfTokenCookie\":[{\"comment\":\"\",\"commentUri\":null,\"httpOnly\":false,\"discard\":false,\"domain\":\"10.41.53.54\",\"expired\":false,\"expires\":\"0001-01-01T00:00:00\",\"name\":\"sap-usercontext\",\"path\":\"/\",\"port\":\"\",\"secure\":false,\"timeStamp\":\"2018-03-06T03:15:44.7516144+00:00\",\"value\":\"sap-client=100\",\"version\":0},{\"comment\":\"\",\"commentUri\":null,\"httpOnly\":false,\"discard\":false,\"domain\":\"10.41.53.54\",\"expired\":false,\"expires\":\"0001-01-01T00:00:00\",\"name\":\"SAP_SESSIONID_TN2_100\",\"path\":\"/\",\"port\":\"\",\"secure\":true,\"timeStamp\":\"2018-03-06T03:15:44.7516467+00:00\",\"value\":\"2E0OgqduK3augOJIXC3-5MIhml8g6xHotZ8AUFa6fLA%3d\",\"version\":0},{\"comment\":\"\",\"commentUri\":null,\"httpOnly\":false,\"discard\":false,\"domain\":\".41.53.54\",\"expired\":false,\"expires\":\"0001-01-01T00:00:00\",\"name\":\"MYSAPSSO2\",\"path\":\"/\",\"port\":\"\",\"secure\":true,\"timeStamp\":\"2018-03-06T03:15:44.7516587+00:00\",\"value\":\"AjQxMDMBABhVADAAMAAwADAAMAAwADAAMAAwADkANwACAAYxADAAMAADABBUAE4AMgAgACAAIAAgACAABAAYMgAwADEAOAAwADMAMAA2ADAAMwAwADkABQAEAAAACAkAAkUA%2fwFWMIIBUgYJKoZIhvcNAQcCoIIBQzCCAT8CAQExCzAJBgUrDgMCGgUAMAsGCSqGSIb3DQEHATGCAR4wggEaAgEBMHAwZDELMAkGA1UEBhMCREUxHDAaBgNVBAoTE1NBUCBUcnVzdCBDb21tdW5pdHkxEzARBgNVBAsTClNBUCBXZWIgQVMxFDASBgNVBAsTC0kwMDIwNzQyNDcyMQwwCgYDVQQDEwNUTjICCAogGAISIiEBMAkGBSsOAwIaBQCgXTAYBgkqhkiG9w0BCQMxCwYJKoZIhvcNAQcBMBwGCSqGSIb3DQEJBTEPFw0xODAzMDYwMzA5MThaMCMGCSqGSIb3DQEJBDEWBBTMtY5CS0GnMIewFv6yYnO1%2fFDv%2fTAJBgcqhkjOOAQDBC4wLAIURzws2FLY6X4N%2fHSWDSxkVEbo2soCFFvWtgBj9VcwUpKxyATGhjXU%2fngb\",\"version\":0}]}";
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.ResponseStatus = ResponseStatus.Completed;
                return response;
            }
            throw new NotImplementedException();
        }
    }
}
