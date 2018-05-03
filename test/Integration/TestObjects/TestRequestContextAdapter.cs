using System;
using Microsoft.AspNetCore.Http;
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

        public IHttpContextAccessor HttpContextAccessor => throw new NotImplementedException();

        public void SetUser(TestUser testUser)
        {
            JWT = testUser.JwtEncodedString;
        }
    }
}
