using PSE.WebAPI.Core.Interfaces;

namespace PSE.Customer.V1.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BPSearchRequest : IAPIRequest
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string MiddleName { get; set; }
    }
}