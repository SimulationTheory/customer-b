using PSE.Customer.V1.Repositories.Entities;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Repositories
{
    public interface IBPByContractAccountRepository
    {
        Task<BPByContractAccountEntity> GetBpByContractAccountId(long contractAccountId);
    }
}