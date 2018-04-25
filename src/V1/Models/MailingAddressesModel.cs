using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Models
{
    /// <summary>
    /// Object Model For Mailing Addresses
    /// </summary>
    public class MailingAddressesModel
    {
        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier.
        /// </value>
        public long AddressID { get; set; }
        /// <summary>
        /// Gets or sets the mailing address.
        /// </summary>
        /// <value>
        /// The mailing address.
        /// </value>
        public Address Address { get; set; }
    }
}
