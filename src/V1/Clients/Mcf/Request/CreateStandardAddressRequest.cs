using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Models;

namespace PSE.Customer.V1.Clients.Mcf.Request
{
    /// <summary>
    ///  Request Object For Create New Address As Standard
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Clients.Mcf.Interfaces.McfBaseAddress" />
    public class CreateStandardAddressRequest : McfBaseAddress
    {
        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier.
        /// </value>
        public long AddressID { get; set; }
    }
}
