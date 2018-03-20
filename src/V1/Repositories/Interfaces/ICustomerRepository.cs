using System.Collections.Generic;
using PSE.Customer.V1.Repositories.Entities;
using System.Threading.Tasks;
using Cassandra;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerEntity> GetCustomerAsync(long bpId);

        Task<CustomerContactEntity> GetCustomerContactAsync(long bpId);

        Task<CustomerEntity> GetCustomerByBusinessPartnerId(long businessPartnerId);

        Task<RowSet> UpdateCustomerMailingAddress(AddressDefinedType address, long bpId);

        Task<RowSet> UpdateCustomerEmailAddress(string emailAddress, long bpId);

        Task<RowSet> UpdateCustomerPhoneNumbers(List<Phone> phones, long bpId);
    }
}
