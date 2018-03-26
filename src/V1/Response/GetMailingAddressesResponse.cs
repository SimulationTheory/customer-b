using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.WebAPI.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{
    /// <summary>
    /// Response Object For Get Mailing Addresses Api
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class GetMailingAddressesResponse : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the mailing address.
        /// </summary>
        /// <value>
        /// The mailing address.
        /// </value>
        public IEnumerable<AddressDefinedType> MailingAddress { get; set; }
    }
}
