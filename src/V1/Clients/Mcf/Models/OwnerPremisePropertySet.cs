using System;

namespace PSE.Customer.V1.Clients.Mcf.Models
{
    public class OwnerPremisePropertySet
    {
        public string Property { get; set; }

        public string Installation { get; set; }

        public string Division { get; set; }

        public DateTimeOffset? Opendate { get; set; }

        public DateTimeOffset? Closedate { get; set; }

        public string Occupiedstatus { get; set; }

        public DateTimeOffset? Lastoccupied { get; set; }

        public DateTimeOffset? Occupiedsince { get; set; }
    }
}
