using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{
    ///
    public class HolidaysInDaterangeResponse : IAPIResponse
    {
        public List<DateTime> Holidays { get; set; }
    }
}