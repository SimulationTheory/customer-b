using System;
using System.Collections.Generic;
using System.Linq;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Repositories.Entities;

namespace PSE.Customer.Extensions
{
    /// <summary>
    /// Extends Customer class
    /// </summary>
    public static class CustomerExtensions
    {

        /// <summary>
        /// Maps some ofthe CustomerEntity fields to CustomerProfileModel object
        /// </summary>
        /// <param name="source"></param>
        /// <returns>CustomerProfileModel</returns>
        /// 
        public static CustomerProfileModel ToModel(this CustomerEntity source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var model = new CustomerProfileModel()
            {
                CustomerName = source.FullName ?? source.FirstName + " " + source.LastName,
                FirstName = source.FirstName,
                LastName = source.LastName,
                MiddleName = GetMiddleName(source.FullName, source.FirstName, source.LastName),
                //TODO: Change after middle_name is added into Cassandra (currently only first_name, last_name, and full_name)
                //source.MiddleName,
                FraudCheck = source.FraudCheck,
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

            List<Phone> phones = GetPhones(source);

            model.Phones = phones;

            model.PrimaryPhone = (model.Phones.Any()) ? model.Phones.First().Type : model.PrimaryPhone;

        }
        //TODO: Delete This Snippet When Middle Name Is Added To Cassandra
        /// <summary>
        /// Gets the name of the middle.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <returns></returns>
        private static string GetMiddleName(string fullName, string firstName, string lastName)
        {
            var middleName = string.Empty;

            if (fullName != null)
            {
                middleName = fullName;

                if (middleName.ToUpper().Contains(firstName.Trim().ToUpper()))
                {
                    middleName = middleName.Replace(firstName, string.Empty);
                }
                if (middleName.ToUpper().Contains(lastName.Trim().ToUpper()))
                {
                    middleName = middleName.Replace(lastName, string.Empty);
                }

            }
            return middleName.Trim();
        }


        /// <summary>
        /// Maps Mcf Fields To Cassandra Fields
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Address McfToCassandraModel(this McfAddressinfo source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var model = new Address
            {
                AddressLine1 = string.IsNullOrEmpty(source.POBox) ? $"{source.HouseNo?.Trim()} {source.Street?.Trim()}" : $"P.O. Box {source.POBox}",
                AddressLine2 = source.HouseNo2?.Trim(),
                CareOf = source.COName?.Trim(),
                City = source.City.Trim(),
                PostalCode = !string.IsNullOrEmpty(source.POBoxPostalCode) ? source.POBoxPostalCode.Trim()
                                                 : source.PostalCode?.Trim(),
                State = source.Region?.Trim(),
                Country = source.CountryID?.Trim(),
                ValidFromDate = source.ValidFromDate?.Trim(),
                ValidToDate = source.ValidToDate?.Trim()
            };

            return model;
        }

        #region private methods
        private static List<Phone> GetPhones(CustomerContactEntity source)
        {
            List<Phone> phones = new List<Phone>();

            foreach (KeyValuePair<string, PhoneDefinedType> entry in source.Phones)
            {
                phones.Add(new Phone { Type = entry.Key.ToEnum<PhoneType>(), Number = entry.Value.Number, Extension = entry.Value.Extension });
            }

            return phones;
        }

        #endregion
    }
}
