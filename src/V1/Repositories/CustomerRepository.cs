using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PSE.Cassandra.Core.Linq;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;

namespace PSE.Customer.V1.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IEntity<CustomerEntity> _customer;
        private readonly IEntity<CustomerContactEntity> _customerContact;
        private readonly ILogger<CustomerRepository> _logger;
        private readonly ISessionFacade<MicroservicesKeyspace> _session;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="customer"></param>
        /// <param name="customerContact"></param>
        /// <param name="logger"></param>
        public CustomerRepository(
            ISessionFacade<MicroservicesKeyspace> session,
            IEntity<CustomerEntity> customer,
            IEntity<CustomerContactEntity> customerContact,
            ILogger<CustomerRepository> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _customer = customer ?? throw new ArgumentNullException(nameof(customer));
            _customerContact = customerContact ?? throw new ArgumentNullException(nameof(customerContact));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        /// <summary>
        ///  Get a given customer based on the bpId
        /// </summary>
        /// <param name="bpId"></param>
        /// <returns>Task<CustomerEntity></returns>
        public async Task<CustomerEntity> GetCustomerAsync(long bpId)
        {
            var customerDetails = _customer.Table;

            var customer = customerDetails.Where(x => x.BpId == bpId).FirstOrDefaultAsync();

            return await customer;
        }

        /// <summary>
        /// Get a given customer contact based on the bpId
        /// </summary>
        /// <param name="bpId"></param>
        /// <returns>Task<CustomerContactEntity></returns>
        public async Task<CustomerContactEntity> GetCustomerContactAsync(long bpId)
        {
            var customerContactDetails = _customerContact.Table;

            var customerContact = customerContactDetails.Where(x => x.BpId == bpId).FirstOrDefaultAsync();

            return await customerContact;
        }
    }
}
