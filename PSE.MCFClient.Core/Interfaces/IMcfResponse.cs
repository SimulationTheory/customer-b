using Newtonsoft.Json;
using PSE.MCFClient.Core.Models;

namespace PSE.MCFClient.Core.Interfaces
{
    public interface IMcfResponse
    {
        [JsonProperty("__metadata")]
        McfMetadata Metadata { get; set; }
    }
}