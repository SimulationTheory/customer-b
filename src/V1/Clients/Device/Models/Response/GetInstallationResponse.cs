using PSE.WebAPI.Core.Interfaces;


namespace PSE.Customer.V1.Clients.Device.Models.Response
{
    /// <summary>
    /// Represents an installation.
    /// </summary>
    /// <seealso cref="PSE.WebAPI.Core.Interfaces.IAPIResponse" />
    public class GetInstallationResponse  : IAPIResponse
    {
        /// <summary>
        /// Gets or sets the installation.
        /// </summary>
        /// <value>
        /// The installation.
        /// </value>
        public Installation Installation { get; set; }
    }
}
