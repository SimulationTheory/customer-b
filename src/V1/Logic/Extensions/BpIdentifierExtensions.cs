using System;
using System.Collections.Generic;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
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
        /// <returns>A list containing all valid identifiers</returns>
        /// <exception cref="ArgumentNullException">source</exception>
        public static List<IdentifierModel> ToModel(this McfResponseResults<BpIdentifier> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = new List<IdentifierModel>();

            if (source.Results != null)
            {
                foreach (var result in source.Results)
                {
                    if (Enum.TryParse(result.IdentifierType, out IdentifierType idType))
                    {
                        list.Add(new IdentifierModel
                        {
                            IdentifierType = idType
                        });
                    }
                }
            }

            return list;
        }
    }
}
