using Microsoft.Extensions.Logging;
using PSE.Cassandra.Core.Linq;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.V1.Repositories.Entities;
using System;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Repositories
{
    public class BPByContractAccountRepository : IBPByContractAccountRepository
    {
        private readonly IEntity<BPByContractAccountEntity> _bpByContractAccountEntity;
        private readonly ILogger<BPByContractAccountRepository> _logger;
        private readonly ISessionFacade<MicroservicesKeyspace> _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="BPByContractAccountRepository"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="bpByContractAccountEntity">The bp by contract account.</param>
        public BPByContractAccountRepository(
            ISessionFacade<MicroservicesKeyspace> session,
            ILogger<BPByContractAccountRepository> logger,
            IEntity<BPByContractAccountEntity> bpByContractAccountEntity)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _bpByContractAccountEntity = bpByContractAccountEntity ?? throw new ArgumentNullException(nameof(bpByContractAccountEntity));
        }

        /// <summary>
        /// Gets the bp by contract account identifier.
        /// </summary>
        /// <param name="contractAccountId">The contract account identifier.</param>
        /// <returns></returns>
        public async Task<BPByContractAccountEntity> GetBpByContractAccountId(long contractAccountId)
        {
            var bpByContractIdTable = _bpByContractAccountEntity.Table;
            var bpByContractIdEntity = await bpByContractIdTable.Where(x => x.ContractAccountId == contractAccountId).FirstOrDefaultAsync();
            return bpByContractIdEntity;
        }
    }
}
