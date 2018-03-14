using PSE.Customer.V1.Models;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Logic.Interfaces
{
    public interface ICustomerLogic
    {
        Task<CustomerProfileModel> GetCustomerProfileAsync(long contractAccountId);
    }
}