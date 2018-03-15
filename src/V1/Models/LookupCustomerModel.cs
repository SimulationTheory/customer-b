using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Models
{
    public class LookupCustomerModel
    {
        public long BPId { get; set; }        
        public bool HasWebAccount { get; set; }
    }
}
