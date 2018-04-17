using PSE.WebAPI.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace PSE.Customer.V1.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class GetInvalidMoveinDatesRequest : IAPIRequest
    {
        /// <summary>
        /// Start date
        /// </summary>
        [Required]
        public DateTime DateFrom{ get; set; }
        /// <summary>
        /// End date
        /// </summary>
        /// 
        [Required]
        public DateTime DateTo { get; set; }
    }
}