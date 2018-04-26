using System;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.Customer.V1.Response;

namespace PSE.Customer.V1.Logic.Extensions
{
    public static class McfStatusCodeResponseExtensions
    {
        public static StatusCodeResponse ToModel(this McfStatusCodeResponse source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var response = new StatusCodeResponse
            {
                ErrorMessage = source.Error?.Message.Value,
                HttpStatusCode = source.HttpStatusCode
            };
            return response;
        }
    }
}
