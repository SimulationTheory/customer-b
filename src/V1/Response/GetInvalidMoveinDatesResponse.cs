using PSE.WebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace PSE.Customer.V1.Response
{
    ///
    public class GetInvalidMoveinDatesResponse : IAPIResponse
    {
        public List<DateTimeOffset> InvalidMoveinDates { get; set; }
    }
}