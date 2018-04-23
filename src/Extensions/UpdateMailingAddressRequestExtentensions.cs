using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Request;
using System;

namespace PSE.Customer.Extensions
{
    /// <summary>
    /// Extension Class For UpdateMailingAddressRequest
    /// </summary>
    public static class UpdateMailingAddressRequestExtentensions
    {
        /// <summary>
        /// To the base.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">source</exception>
        public static AddressDefinedType ToBase(this UpdateMailingAddressRequest source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var model = new AddressDefinedType
            {
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                CareOf = source.CareOf,
                City = source.City,
                Country = source.Country,
                PostalCode = source.PostalCode,
                State = source.State
            };

            return model;
        }
    }
}
