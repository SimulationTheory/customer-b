using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PSE.Customer.V1.Repositories.DefinedTypes;


namespace PSE.Customer.V1.Models
{
    public class IdentifierModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public IdentifierType IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
    }
}
