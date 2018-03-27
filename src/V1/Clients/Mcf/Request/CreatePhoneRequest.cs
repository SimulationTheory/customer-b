namespace PSE.Customer.V1.Clients.Mcf.Request
{
    /// <summary>
    /// Parameters for POSTing phone data
    /// </summary>
    /// <remarks>
    /// This class includes location data.  If both location dependent and independent phones can be posted successfully,
    /// the other class goes away - otherwise this comment should be updated.
    /// </remarks>
    public class CreatePhoneRequest
    {
        /// <summary>
        /// Gets or sets the business partner identifier.
        /// </summary>
        /// <value>
        /// The business partner identifier.
        /// </value>
        public long AccountID { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is home.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is home; otherwise, <c>false</c>.
        /// </value>
        public bool HomeFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is standard.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is standard; otherwise, <c>false</c>.
        /// </value>
        public bool StandardFlag { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// Optional phone number extension.
        /// </value>
        public string Extension { get; set; }
    }
}
