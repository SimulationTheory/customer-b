using Newtonsoft.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.RestUtility.Core.Interfaces
{
    public interface IMcfDeferred
    {
        [JsonProperty("__deferred")]
        McfDeferred Deferred { get; set; }
    }
}
