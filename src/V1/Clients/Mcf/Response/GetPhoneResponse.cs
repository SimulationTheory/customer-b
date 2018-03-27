using PSE.Customer.V1.Clients.Mcf.Request;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Returned when phone data is POSTed
    /// </summary>
    public class GetPhoneResponse : CreatePhoneRequest, IMcfResult
    {
        /// <summary>
        /// Sequence number
        /// </summary>
        public string SequenceNo { get; set; }

        /// <summary>
        /// Phone number, not sure how this is different from the other phone number field
        /// </summary>
        public string CompletePhoneNo { get; set; }

        /// <summary>
        /// Country code, appears to be standard two letter abbreviation (e.g. US)
        /// </summary>
        public string CountryID { get; set; }

        /// <summary>
        /// Numeric code that should map to home, work, mobile (mapping TBD)
        /// </summary>
        public string PhoneType { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata is ignored
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}