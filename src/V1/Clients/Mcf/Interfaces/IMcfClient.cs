using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Interfaces
{
    public interface IMcfClient
    {
        McfResponse<CreateAddressIndependantPhoneResponse> CreateBusinessPartnerLevelPhone(string jwt, CreateAddressIndependantPhoneRequest request);
    }
}