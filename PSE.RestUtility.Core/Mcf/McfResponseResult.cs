using System.Collections.Generic;
using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;

namespace PSE.RestUtility.Core.Mcf
{
    public class McfResponseResult<T> : IMcfResult
    {
        public McfMetadata Metadata { get; set; }
        public T Result{ get; set; }
    }
}
