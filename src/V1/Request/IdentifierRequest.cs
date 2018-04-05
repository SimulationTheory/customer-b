using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.WebAPI.Core.Interfaces;
using System;

namespace PSE.Customer.V1.Request
{
    public class IdentifierRequest : IAPIRequest
    {
        public string AccountID { get; set; }
        public IdentifierType Identifiertype { get; set; }
        public DateTime Identrydate { get; set; }
        public string Identifierno { get; set; }
        public DateTime Idvalidfromdate { get; set; }
        public DateTime Idvalidtodate { get; set; }

    }
}