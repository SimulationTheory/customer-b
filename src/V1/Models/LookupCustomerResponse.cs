using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Models
{
    public class LookupCustomerResponse: IAPIResponse
    {
        public string BPId { get; set; }        
        public bool HasWebAccount { get; set; }
    }
}
