using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.Extensions.Logging;
using PSE.Cassandra.Core.Linq;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
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

            var customer = customerDetails.Where(x => x.BusinessPartnerId == bpId).FirstOrDefaultAsync();

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

        /// <summary>
        /// Gets the customer by business partner identifier.
        /// </summary>
        /// <param name="businessPartnerId">The business partner identifier.</param>
        /// <returns></returns>
        public async Task<CustomerEntity> GetCustomerByBusinessPartnerId(long businessPartnerId)
        {
            var customerTable = _customer.Table;
            var customerEntity = await customerTable.Where(x => x.BusinessPartnerId == businessPartnerId).FirstOrDefaultAsync();
            return customerEntity;
        }

        /// <summary>
        /// Updates the customers mailing address at the BP level
        /// </summary>
        /// <param name="address">mailing address</param>
        /// <param name="bpId">bpID</param>
        public Task<RowSet> UpdateCustomerMailingAddress(AddressDefinedType address, long bpId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the customers email address at the BP level
        /// </summary>
        /// <param name="emailAddress">email address</param>
        /// <param name="bpId">bpID</param>
        public Task<RowSet> UpdateCustomerEmailAddress(string emailAddress, long bpId)
        {
            var session = _session.Session();
            var statement = session.Prepare(
                "UPDATE customer_contact " +
                "SET email = ? " +
                "WHERE bp_id = ?;");

            return session.ExecuteAsync(statement.Bind(emailAddress, bpId));
        }

        /// <summary>
        /// Updates the phone numbers at the BP level
        /// </summary>
        /// <param name="phones">all phone numbers for the customer</param>
        /// <param name="bpId">bpID</param>
        public Task<RowSet> UpdateCustomerPhoneNumbers(List<Phone> phones, long bpId)
        {
            throw new NotImplementedException();
        }
    }
}
