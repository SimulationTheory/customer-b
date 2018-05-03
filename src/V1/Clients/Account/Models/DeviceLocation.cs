using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.V1.Clients.Account.Models
{
    public class DeviceLocation
    {
        public long DeviceLocationId { get; set; }
        public string MeterId { get; set; }
        public bool? DisconnectedIndicator { get; set; }
        public string ServiceType { get; set; }
        public AddressDefinedType ServiceAddress { get; set; }
    }
}
