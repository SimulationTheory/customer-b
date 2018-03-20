using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace PSE.RestUtility.Core.Models
{
    public class TokenCookies
    {
        [JsonProperty("mcfTokenCookie")]
        public IEnumerable<Cookie> Result { get; set; }
    }
}