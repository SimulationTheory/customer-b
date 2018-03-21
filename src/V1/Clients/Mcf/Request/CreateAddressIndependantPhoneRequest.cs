using Newtonsoft.Json;
using PSE.RestUtility.Core.Json;

namespace PSE.Customer.V1.Clients.Mcf.Request
{
    public class CreateAddressIndependantPhoneRequest
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        [JsonProperty("AccountID")]
        public long BusinessPartnerId { get; set; }
        [JsonProperty("PhoneNo")]
        public string PhoneNumber { get; set; }
        [JsonProperty("HomeFlag")]
        public bool IsHome { get; set; }
        [JsonProperty("StandardFlag")]
        public bool IsStandard { get; set; }
    }
}