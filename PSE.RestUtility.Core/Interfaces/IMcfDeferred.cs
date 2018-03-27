using Newtonsoft.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.RestUtility.Core.Interfaces
{
    /// !!! This class is not an MCF clas and should be in the V1/Clients/Mcf folders.  Not sure if this is needed.
    public interface IMcfDeferred
    {
        [JsonProperty("__deferred")]
        McfDeferred Deferred { get; set; }
    }
}
