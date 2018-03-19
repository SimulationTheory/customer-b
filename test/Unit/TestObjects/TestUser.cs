using System.Collections.Generic;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;

namespace PSE.Customer.Tests.Unit.TestObjects
{
    public class TestUser
    {
        public TestUser()
        {
            Phones = new List<Phone>();
        }

        public long ContractAccountId { get; set; }

        public long BpNumber { get; set; }

        public string JwtToken { get; set; }

        public string Username { get; set; }

        public AddressDefinedType Address { get; set; }

        public string Email { get; set; }

        public List<Phone> Phones { get; set; }
    }
}
