using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PSE.Cassandra.Core.Linq;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Customer.V1.Repositories.Views;

namespace PSE.Customer.V1.Repositories
{
    public class ContractAccountRepository : IContractAccountRepository
    {
        private readonly IEntity<ContractAccountEntity> _accountSession;
        private readonly IEntity<ContractAccountByBusinessPartnerView> _bpSession;
        private readonly ILogger<ContractAccountRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractAccountRepository"/> class.
        /// </summary>
        /// <param name="accountSession">The account session.</param>
        /// <param name="bpSession">The bp session.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// accountSession
        /// or
        /// bpSession
        /// or
        /// logger
        /// </exception>
        public ContractAccountRepository(IEntity<ContractAccountEntity> accountSession, IEntity<ContractAccountByBusinessPartnerView> bpSession, ILogger<ContractAccountRepository> logger)
        {
            _accountSession = accountSession ?? throw new ArgumentNullException(nameof(accountSession));
            _bpSession = bpSession ?? throw new ArgumentNullException(nameof(bpSession));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the business partner identifier by contract account id.
        /// </summary>
        /// <param name="contractAccountId">The contract account identifier.</param>
        /// <returns></returns>
        public Task<ContractAccountByBusinessPartnerView> GetBusinessPartnerIdByContractAccount(long contractAccountId)
        {
            var contractAccountView = _bpSession.Table;
            var bpAccount = contractAccountView.Where(x => x.ContractAccountId == contractAccountId);

            return bpAccount.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the contract account.
        /// </summary>
        /// <param name="businessPartnerId">The business partner identifier.</param>
        /// <param name="contractAccountId">The contract account identifier.</param>
        /// <returns></returns>
        public Task<ContractAccountEntity> GetContractAccount(long businessPartnerId, long contractAccountId)
        {
            var contractAccounts = _accountSession.Table;

            // TODO: add FirstOrDefaultAsync() that accepts predicate to PSE.Cassandra.Core.Linq.QueryableExtensions
            var account = contractAccounts.Where(x => x.BusinessPartnerId == businessPartnerId 
                                                   && x.ContractAccountId == contractAccountId);

            return account.FirstOrDefaultAsync();
        }
    }
}