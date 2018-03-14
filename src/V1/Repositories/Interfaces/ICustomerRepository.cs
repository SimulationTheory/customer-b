using PSE.Customer.V1.Repositories.Entities;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerEntity> GetCustomerAsync(long bpId);
        Task<CustomerContactEntity> GetCustomerContactAsync(long bpId);
    }
}
