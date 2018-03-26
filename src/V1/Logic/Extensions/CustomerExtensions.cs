using PSE.Customer.Extensions;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Repositories.Entities;
using PSE.RestUtility.Core.Mcf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSE.Customer.V1.Logic
{
    /// <summary>
    /// Extends Customer class
    /// </summary>
    public static  class CustomerExtensions
    {
        /// <summary>
        /// Maps some ofthe CustomerEntity fields to CustomerProfileModel object
        /// </summary>
        /// <param name="source"></param>
        /// <returns>CustomerProfileModel</returns>
        public static CustomerProfileModel ToModel(this CustomerEntity source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var model = new CustomerProfileModel()
            {
                CustomerName = source.FullName ?? source.FirstName + " " + source.LastName,
                OrganizationName = source.EmployerName,
                IsPva = source.PvaIndicator,
            };

            return model;
        }

        /// <summary>
        /// Augment the CustomerProfileModel object with some of the CustomerContactEntity fields
        /// </summary>
        /// <param name="source"></param>
        /// <param name="model"></param>
        public static void AddToModel(this CustomerContactEntity source, CustomerProfileModel model)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            model.EmailAddress = source.Email;
            model.MailingAddress = source.MailingAddress;

            var phones = GetPhones(source);
            model.Phones = phones;
            model.PrimaryPhone = (model.Phones.Any()) ? model.Phones.First().Type : model.PrimaryPhone;
        }

        /// <summary>
        /// Maps Mcf Fields To Cassabdra Fields
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static AddressDefinedType McfToCassandraModel(this McfAddressinfo source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var model = new AddressDefinedType
            {
                AddressLine1 = source.Street,
                AddressLine2 = null,
                City = source.City,
                PostalCode = source.PostalCode,
                State = source.Region,
                Country = source.CountryName
            };

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<AddressDefinedType> ToModels(this IEnumerable<McfAddressinfo> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var models = source.ToArray().Select(x => x.McfToCassandraModel());

            return models;
        }

        private static List<Phone> GetPhones(CustomerContactEntity source)
        {
            var phones = new List<Phone>();

            foreach (KeyValuePair<string, PhoneDefinedType> entry in source.Phones)
            {
                phones.Add(new Phone { Type = entry.Key.ToEnum<PhoneType>(), Number = entry.Value.Number, Extension = entry.Value.Extension });
            }

            return phones;
        }
    }
}
