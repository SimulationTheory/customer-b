using PSE.Customer.V1.Clients.ClientProxy.Interfaces;

namespace PSE.Customer.V1.Clients.ClientProxy
{
    public class ApiUser : IApiUser
    {
        /// <inheritdoc />
        public string Username { get; set; }

        /// <inheritdoc />
        public long ContractAccountId { get; set; }

        /// <inheritdoc />
        public long BPNumber { get; set; }

        /// <inheritdoc />
        public string JwtToken { get; set; }
    }
}
