using System;
using System.Collections.Generic;
using System.Globalization;
using PSE.Customer.Extensions;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Models;

namespace PSE.Customer.V1.Logic.Extensions
{
    public static class OwnerAccountsExtensions
    {
        public static IEnumerable<OwnerAccountModel> ToModels(this OwnerPremiseSet source, string contractAccountId)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var premises = new List<OwnerAccountModel>();

            foreach (var premiseProperty in source.OwnerPremiseProperty.Results)
            {
                var occupiedDate = premiseProperty.Occupiedsince ?? premiseProperty.Lastoccupied;

                var model = new OwnerAccountModel
                {
                    ServiceAddress = source.PremiseAddress.McfToCassandraModel(),
                    ContractAccountNumber = contractAccountId,
                    OccupiedStatus = premiseProperty.Occupiedstatus,
                };
                if (occupiedDate.HasValue)
                {
                    model.StatusDate = occupiedDate.Value.ToString("d", CultureInfo.InvariantCulture);
                }

                premises.Add(model);
            }

            return premises;
        }

        public static IEnumerable<OwnerAccountModel> ToModels(this OwnerAccountsSet source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var ownerAccounts = new List<OwnerAccountModel>();

            foreach (var ownerContractAccount in source.OwnerContractAccount.Results)
            {
                foreach (var set in ownerContractAccount.OwnerPremise.Results)
                {
                    ownerAccounts.AddRange(set.ToModels(ownerContractAccount.ContractAccount));
                }
            }

            return ownerAccounts;
        }

        public static IEnumerable<OwnerAccountModel> ToModels(this IEnumerable<OwnerAccountsSet> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var results = new List<OwnerAccountModel>();

            foreach (var item in source)
            {
                results.AddRange(item.ToModels());
            }

            return results;
        }
    }
}
