using Newtonsoft.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.RestUtility.Core.Interfaces
{
    public interface IMcfResult
    {
        [JsonProperty("__metadata")]
        McfMetadata Metadata { get; set; }
    }
}