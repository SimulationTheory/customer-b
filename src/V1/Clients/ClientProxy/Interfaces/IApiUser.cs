namespace PSE.Customer.V1.Clients.ClientProxy.Interfaces
{
    public interface IApiUser
    {
        /// <summary>
        /// Username
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Contract account ID
        /// </summary>
        long ContractAccountId { get; set; }

        /// <summary>
        /// Business partner number
        /// </summary>
        long BPNumber { get; set; }

        /// <summary>
        /// Token that is set when the user logs in
        /// </summary>
        string JwtToken { get; set; }
    }
}
