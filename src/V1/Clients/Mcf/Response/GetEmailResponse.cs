using Newtonsoft.Json;
using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Json;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Returned from POST or GET of an email.
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Clients.Mcf.Request.CreateEmailRequest" />
    public class GetEmailResponse : CreateEmailRequest, IMcfResult
    {
        /// <summary>
        /// Gets or sets the sequence number
        /// </summary>
        /// <value>
        /// Three digit sequence number, presumably defining order of records returned from SAP
        /// </value>
        [JsonConverter(typeof(ToStringJsonConverter))]
        public string SequenceNo { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}
