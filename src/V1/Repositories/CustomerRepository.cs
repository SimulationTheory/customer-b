using System;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.Extensions.Logging;
using PSE.Cassandra.Core.Linq;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;

namespace PSE.Customer.V1.Repositories
{
    /// <summary>
    /// Repository For Customer
    /// </summary>
    /// <seealso cref="PSE.Customer.V1.Repositories.Interfaces.ICustomerRepository" />
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IEntity<CustomerEntity> _customer;
        private readonly IEntity<CustomerContactEntity> _customerContact;
        private readonly ILogger<CustomerRepository> _logger;
        private readonly ISessionFacade<MicroservicesKeyspace> _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="customer">The customer.</param>
        /// <param name="customerContact">The customer contact.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">
        /// session
        /// or
        /// customer
        /// or
        /// customerContact
        /// or
        /// logger
        /// </exception>
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
        /// <returns>Task returning CustomerEntity</returns>
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
        /// <returns>Task returning CustomerEntity</returns>
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
        /// <returns>Task returning CustomerEntity</returns>
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
            var session = _session.Session();
            var statement = session.Prepare(
                "UPDATE customer_contact " +
                "SET mailing_address = ? " +
                "WHERE bp_id = ?;");

            return session.ExecuteAsync(statement.Bind(address, bpId));
        }

        /// <summary>
        /// Updates the customers email address at the BP level
        /// </summary>
        /// <param name="emailAddress">email address</param>
        /// <param name="bpId">bpID</param>
        public Task<RowSet> UpdateCustomerEmailAddress(string emailAddress, long bpId)
        {
            _logger.LogInformation($"UpdateCustomerEmailAddress({nameof(emailAddress)}: {emailAddress}," +
                                   $"{nameof(bpId)}: {bpId})");

            var session = _session.Session();
            var statement = session.Prepare(
                "UPDATE customer_contact " +
                "SET email = ? " +
                "WHERE bp_id = ?;");

            return session.ExecuteAsync(statement.Bind(emailAddress, bpId));
        }

        /// <summary>
        /// Saves the cell phone number at the BP level
        /// </summary>
        /// <param name="phone">Customer's cell phone</param>
        /// <param name="bpId">Business partner ID</param>
        /// <returns></returns>
        public async Task<RowSet> UpdateCustomerPhoneNumber(Phone phone, long bpId)
        {
            _logger.LogInformation($"UpdateCustomerPhoneNumber({nameof(phone)}: {phone.ToJson()}," +
                                   $"{nameof(bpId)}: {bpId})");

            // Get existing phone data
            var customerContact = await GetCustomerContactAsync(bpId);
            var phoneTypeName = phone.Type.GetEnumMemberValue();
            string transactionType;

            // Insert or update dictionary
            if (customerContact.Phones.ContainsKey(phoneTypeName))
            {
                var phoneEntry = customerContact.Phones[phoneTypeName];
                transactionType = "Updating";
                phoneEntry.Number = phone.Number;
                phoneEntry.Extension = phone.Extension;
            }
            else
            {
                transactionType = "Adding";
                customerContact.Phones.Add(phoneTypeName, new PhoneDefinedType
                {
                    Number = phone.Number,
                    Extension = phone.Extension
                });
            }

            // Persist to Cassandra
            var session = _session.Session();
            var statement = session.Prepare(
                "UPDATE customer_contact " +
                "SET phones = ? " +
                "WHERE bp_id = ?;");

            _logger.LogInformation($"{transactionType} phone({nameof(phone.Number)}: {phone.Number}," +
                                   $"{nameof(phone.Extension)}: {phone.Extension})");
            return await session.ExecuteAsync(statement.Bind(customerContact.Phones, bpId));
        }
    }
}
