using System.Threading.Tasks;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Views;

namespace PSE.Customer.V1.Repositories.Interfaces
{
    public interface IContractAccountRepository
    {
        Task<ContractAccountByBusinessPartnerView> GetBusinessPartnerIdByContractAccount(long contractAccountId);
        Task<ContractAccountEntity> GetContractAccount(long  businessPartnerId, long contractAccountId);
    }
}