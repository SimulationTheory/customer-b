using System;
using PSE.WebAPI.Core.Service.Enums;
using PSE.WebAPI.Core.Service.Interfaces;

namespace PSE.Customer.Tests.Integration.TestObjects
{
    public class TestRequestContextAdapter : IRequestContextAdapter
    {
        public TestRequestContextAdapter()
        {
            RequestChannel = RequestChannelEnum.Web;
        }

        public string JWT { get; set; }

        public RequestChannelEnum RequestChannel { get; set; }

        public Guid UserId { get; set; }

        public void SetUser(TestUser testUser)
        {
            JWT = testUser.JwtEncodedString;
        }
    }
}
