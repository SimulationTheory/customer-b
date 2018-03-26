using System.Collections.Generic;
using PSE.Customer.V1.Clients.ClientProxy;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.Tests.Integration.TestObjects
{
    public class TestUser : ApiUser
    {
        public TestUser()
        {
            Phones = new List<Phone>();
        }

        public TestUser(string jwtEncodedString) : base(jwtEncodedString)
        {
        }

        public AddressDefinedType Address { get; set; }

        public string Email { get; set; }

        public List<Phone> Phones { get; set; }
    }
}
