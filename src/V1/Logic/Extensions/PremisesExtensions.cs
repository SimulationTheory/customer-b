using System;
using System.Collections.Generic;
using System.Globalization;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Models;

namespace PSE.Customer.V1.Logic.Extensions
{
    public static class PremisesExtensions
    {
        public static IEnumerable<PremiseModel> ToModels(this PremisesSet source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var premises = new List<PremiseModel>();

            foreach (var installation in source.Installations.Results)
            {
                var premise = new PremiseModel
                {
                    ServiceAddress = source.Address.McfToCassandraModel(),
                    InstallationAccountNumber = installation.InstallationId,
                    ContractAccountStartDate = installation.MoveInDateFrom.ToString(),
                    ContractAccountEndDate = string.Format("{0:d}", installation.MoveInDateTo)
                };
                if (installation.MoveInDateFrom.HasValue)
                {
                    premise.ContractAccountStartDate = installation.MoveInDateFrom.Value.ToString("d", CultureInfo.InvariantCulture);
                }
                if (installation.MoveInDateTo.HasValue)
                {
                    premise.ContractAccountEndDate = installation.MoveInDateTo.Value.ToString("d", CultureInfo.InvariantCulture);
                }
                premises.Add(premise);
            }

            return premises;
        }

        public static IEnumerable<PremiseModel> ToModels(this IEnumerable<PremisesSet> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var results = new List<PremiseModel>();

            foreach (var item in source)
            {
                results.AddRange(item.ToModels());
            }

            return results;
        }
    }
}
