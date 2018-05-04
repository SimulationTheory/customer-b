using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Clients.Account.Models.Response
{
    /// <summary>
    /// CreateAccountResponse Model
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class CreateAccountResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the business partner identifier.
        /// </summary>
        /// <value>
        /// The business partner identifier.
        /// </value>
        public long BusinessPartnerId { get; set; }

        /// <summary>
        /// Gets or sets the contract account identifier.
        /// </summary>
        /// <value>
        /// The contract account identifier.
        /// </value>
        public long ContractAccountId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier.
        /// </value>
        public long AddressID { get; set; }

        /// <summary>
        /// Gets or sets the bill to account identifier.
        /// </summary>
        /// <value>
        /// The bill to account identifier.
        /// </value>
        public long BillToAccountID { get; set; }
    }
}
