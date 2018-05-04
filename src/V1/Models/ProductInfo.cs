using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSE.Customer.V1.Models
{
    public class ProductInfo
    {
        /// <summary>
        /// Product type ex Green, Solar, Carbon
        /// </summary>
        public string ProductType { get; set; }
        public decimal EnrollmentAmount { get; set; }
    }
}
