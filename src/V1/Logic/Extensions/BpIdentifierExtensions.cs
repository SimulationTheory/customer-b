using System;
using System.Collections.Generic;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Response;
using PSE.RestUtility.Core.Mcf;

namespace PSE.Customer.V1.Logic.Extensions
{
    /// <summary>
    /// Helper extension methods for conversions
    /// </summary>
    public static class BpIdentifierExtensions
    {
        /// <summary>
        /// Converts a BpIdentifier response to the corresponding model.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The converted model object</returns>
        /// <exception cref="ArgumentNullException">source</exception>
        public static IdentifierResponse ToModel(this BpIdentifier source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var response = new IdentifierResponse
            {
                BpId = source.AccountId,
                IdentifierNo = source.IdentifierNo
            };

            if (Enum.TryParse(source.IdentifierType, out IdentifierType idType))
            {
                response.IdentifierType = idType;
            }

            if (DateTime.TryParse(source.IdEntryDate, out DateTime entryDate))
            {
                response.IdEntryDate = entryDate;
            }

            return response;
        }

        /// <summary>
        /// Converts a BpIdentifier response to the corresponding model.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>A list containing all valid identifiers</returns>
        /// <exception cref="ArgumentNullException">source</exception>
        public static List<IdentifierType> ToModel(this McfResponseResults<BpIdentifier> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = new List<IdentifierType>();

            if (source.Results != null)
            {
                foreach (var result in source.Results)
                {
                    if (Enum.TryParse(result.IdentifierType, out IdentifierType idType))
                    {
                        list.Add(idType);
                    }
                }
            }

            return list;
        }
    }
}
