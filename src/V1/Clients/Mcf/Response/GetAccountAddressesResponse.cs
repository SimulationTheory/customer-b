using Newtonsoft.Json;
using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    /// <summary>
    /// Response Object For Get All Addresses For A Bp
    /// </summary>
    public class GetAccountAddressesResponse : IMcfResult
    {
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>
        /// The account identifier.
        /// </value>
        public long AccountID { get; set; }
        /// <summary>
        /// Gets or sets the address identifier.
        /// </summary>
        /// <value>
        /// The address identifier.
        /// </value>
        public long AddressID { get; set; }
        /// <summary>
        /// Gets or sets the address information.
        /// </summary>
        /// <value>
        /// The address information.
        /// </value>
        public McfAddressinfo AddressInfo { get; set; }
        /// <summary>
        /// Gets or sets the account address dependent faxes.
        /// </summary>
        /// <value>
        /// The account address dependent faxes.
        /// </value>
        public McfAccountAddressDependentFaxes AccountAddressDependentFaxes { get; set; }
        /// <summary>
        /// Gets or sets the account address dependent mobile phones.
        /// </summary>
        /// <value>
        /// The account address dependent mobile phones.
        /// </value>
        public McfAccountAddressDependentMobilePhones AccountAddressDependentMobilePhones { get; set; }
        /// <summary>
        /// Gets or sets the account address dependent phones.
        /// </summary>
        /// <value>
        /// The account address dependent phones.
        /// </value>
        public McfAccountAddressDependentPhones AccountAddressDependentPhones { get; set; }
        /// <summary>
        /// Gets or sets the account address usages.
        /// </summary>
        /// <value>
        /// The account address usages.
        /// </value>
        public McfAccountAddressUsages AccountAddressUsages { get; set; }
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public McfMetadata Metadata { get; set; }
    }
}