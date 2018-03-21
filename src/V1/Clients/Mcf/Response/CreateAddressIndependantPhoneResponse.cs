using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    public class CreateAddressIndependantPhoneResponse : IMcfResult
    {
        [JsonProperty("AccountID")]
        public long BusinessPartnerId { get; set; }
        [JsonProperty("SequenceNo")]
        public string SequenceNumber { get; set; }
        [JsonProperty("PhoneNo")]
        public string PhoneNumber { get; set; }
        [JsonProperty("HomeFlag")]
        public bool IsHome { get; set; }
        [JsonProperty("StandardFlag")]
        public bool IsStandard { get; set; }
        [JsonProperty("Extension")]
        public string Extension { get; set; }
        [JsonProperty("CompletePhoneNo")]
        public string CompletePhoneNumber { get; set; }
        [JsonProperty("CountryID")]
        public string CountryId { get; set; }
        [JsonProperty("PhoneType")]
        public string PhoneType { get; set; }
        public  McfMetadata Metadata { get; set; }
    }
}