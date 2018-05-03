using PSE.WebAPI.Core.Interfaces;
using System.Collections.Generic;

namespace PSE.Customer.V1.Clients.Account.Models.Response
{
    /// <summary>
    /// Represents a list of contract items.
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class GetContractItemsResponse : IAPIResponse 
    {
        /// <summary>
        /// Gets or sets the contract items.
        /// </summary>
        public List<GetContractItemResponse> ContractItems { get; set; }

    }
}
