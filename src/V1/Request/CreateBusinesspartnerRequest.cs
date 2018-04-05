using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Request
{
    public class CreateBusinesspartnerRequest : IAPIRequest
    {
        public string Channel { get; set; }
        public string PartnerCategory { get; set; }
        public string PartnerRole { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string OrgName { get; set; }
        public string CorrLanguage { get; set; }
        public string Email { get; set; }
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
        public string POBox { get; set; }
    }
}