

using PSE.RestUtility.Core.Interfaces;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Clients.Mcf.Response
{
    public class CreateBusinessPartnerMcfResponse : IMcfResult
    { 
        public McfMetadata Metadata { get; set; }
        public string Channel { get; set; }
        public string Extension { get; set; }
        public string EMailValidFrom { get; set; }
        public string PartnerCategory { get; set; }
        public string PhoneValidFrom { get; set; }
        public string PartnerRole { get; set; }
        public string Taxjurcode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string OrgName { get; set; }
        public string CorrLanguage { get; set; }
        public string EMail { get; set; }
        public string Phone { get; set; }
        public string PhoneType { get; set; }
        public string AddressType { get; set; }
        public string CareOf { get; set; }
        public string HouseNum { get; set; }
        public string Street { get; set; }
        public string Supplement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PoBox { get; set; }
        public string Country { get; set; }
        public string PartnerId { get; set; }
        public string Addrnumber { get; set; }
        public string Status { get; set; }
        public string Msg { get; set; }
    }
       
}
