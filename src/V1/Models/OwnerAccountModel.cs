namespace PSE.Customer.V1.Models
{
    public class OwnerAccountModel
    {
        /// <summary>
        /// Gets or sets the service address.
        /// </summary>
        /// <value>
        /// The service address.
        /// </value>
        public Address ServiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the contract account number.
        /// </summary>
        /// <value>
        /// The contract account number.
        /// </value>
        public string ContractAccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the occupied status.
        /// </summary>
        /// <value>
        /// The occupied status (e.g. "Occupied", "Vacant").
        /// </value>
        public string OccupiedStatus { get; set; }

        /// <summary>
        /// Gets or sets the date the property was occupied or vacant.
        /// </summary>
        /// <value>
        /// The date the property was occupied or vacant.
        /// </value>
        public string StatusDate { get; set; }
    }
}
