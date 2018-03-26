using System.Collections.Generic;
using PSE.Customer.V1.Repositories.Entities;
using System.Threading.Tasks;
using Cassandra;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Repositories.Interfaces
{
    /// <summary>
    /// Interface For Customer Repository
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Gets the customer asynchronous.
        /// </summary>
        /// <param name="bpId">The bp identifier.</param>
        /// <returns></returns>
        Task<CustomerEntity> GetCustomerAsync(long bpId);
        /// <summary>
        /// Gets the customer contact asynchronous.
        /// </summary>
        /// <param name="bpId">The bp identifier.</param>
        /// <returns></returns>
        Task<CustomerContactEntity> GetCustomerContactAsync(long bpId);
        /// <summary>
        /// Gets the customer by business partner identifier.
        /// </summary>
        /// <param name="businessPartnerId">The business partner identifier.</param>
        /// <returns></returns>
        Task<CustomerEntity> GetCustomerByBusinessPartnerId(long businessPartnerId);
        /// <summary>
        /// Updates the customer mailing address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="bpId">The bp identifier.</param>
        /// <returns></returns>
        Task<RowSet> UpdateCustomerMailingAddress(AddressDefinedType address, long bpId);
        /// <summary>
        /// Updates the customer email address.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="bpId">The bp identifier.</param>
        /// <returns></returns>
        Task<RowSet> UpdateCustomerEmailAddress(string emailAddress, long bpId);
        /// <summary>
        /// Updates the customer phone numbers.
        /// </summary>
        /// <param name="phones">The phones.</param>
        /// <param name="bpId">The bp identifier.</param>
        /// <returns></returns>
        Task<RowSet> UpdateCustomerPhoneNumbers(List<Phone> phones, long bpId);
    }
}
