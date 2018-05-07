namespace PSE.Customer.V1.Models
{
    public class PremiseModel
    {
        /// <summary>
        /// Gets or sets the service address.
        /// </summary>
        /// <value>
        /// The service address.
        /// </value>
        public Address ServiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the installation account number.
        /// </summary>
        /// <value>
        /// The installation account number.
        /// </value>
        public string InstallationAccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the contract account start date.
        /// </summary>
        /// <value>
        /// The contract account start date.
        /// </value>
        public string ContractAccountStartDate { get; set; }

        /// <summary>
        /// Gets or sets the contract account end date.
        /// </summary>
        /// <value>
        /// The contract account end date.
        /// </value>
        public string ContractAccountEndDate { get; set; }
    }
}
