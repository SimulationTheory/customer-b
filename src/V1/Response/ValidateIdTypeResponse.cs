namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Returns whether or not the validation check matched the existing identifier
    /// </summary>
    public class ValidateIdTypeResponse
    {
        /// <summary>
        /// Gets or sets the PII match.
        /// </summary>
        public bool PiiMatch { get; set; }
    }
}
