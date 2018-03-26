namespace PSE.Customer.V1.Clients.Mcf.Request
{
    /// <summary>
    /// Creates or updates an email address
    /// </summary>
    public class CreateEmailRequest
    {
        /// <summary>
        /// Gets or sets the business partner identifier.
        /// </summary>
        /// <value>
        /// The business partner identifier.
        /// </value>
        public string AccountID { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

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
    }
}
