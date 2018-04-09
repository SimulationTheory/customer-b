using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            List<Phone> phones = GetPhones(source);

            model.Phones = phones;

            model.PrimaryPhone = (model.Phones.Any()) ? model.Phones.First().Type : model.PrimaryPhone;

        }


        /// <summary>
        /// Maps Mcf Fields To Cassandra Fields
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
                AddressLine1 = string.IsNullOrEmpty(source.POBox) ? $"{source.HouseNo?.Trim()} {source.Street.Trim()}" :  source.POBox,
                AddressLine2 =  source?.HouseNo2?.Trim(), 
                City = source.City.Trim(),
                PostalCode = !string.IsNullOrEmpty(source.POBoxPostalCode) ? source.POBoxPostalCode.Trim()
                                                 : source.PostalCode.Trim(),
                State = source.Region.Trim(),
                Country = source.CountryID.Trim()
            };

            return model;
        }

        /// <summary>        
        /// Maps Cassandra Fields To Mcf Fields
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static McfAddressinfo CassandraToMcfModel(this AddressDefinedType source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var model = new McfAddressinfo
            {
                POBox = ParsePOBOX(source.AddressLine1, source.AddressLine2),
                Street = ParseStreet(source.AddressLine1, ParseHouseNumber(source.AddressLine1)),
                HouseNo = ParseHouseNumber(source.AddressLine1),
                HouseNo2 = source.AddressLine2.Trim(),
                PostalCode = source.PostalCode.Trim(),
                POBoxPostalCode = source.PostalCode.Trim(),
                City = source.City.Trim(),
                Region = source.State.Trim(),
                CountryID = source.Country.Trim()
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


        //TODO : Move The Logic To Address Microservice For Centralization
        private static string ParsePOBOX(string AddressLine1, string AddressLine2)
        {
            //validate and extract a po box  

            string sPoBoxNoPOB = "[Pp]\\.?\\s*?[Oo]\\.?\\s*[Bb][Oo][Xx]";
            string sPoBoxReplace = "[Pp]\\.?\\s*?[Oo]\\.?\\s*[Bb][Oo][Xx]\\s*";
            string sPoBoxValidateMax = "[Pp]\\.?\\s*?[Oo]\\.?\\s*[Bb][Oo][Xx]\\s*[1-9][0-9a-zA-Z]{10}";
            string sPoBoxValidateMin = "[Pp]\\.?\\s*?[Oo]\\.?\\s*[Bb][Oo][Xx]\\s*[1-9]";
            string sPoBoxExtract = "[Pp]\\.?\\s*?[Oo]\\.?\\s*[Bb][Oo][Xx]\\s*[1-9][0-9a-zA-Z]{0,9}";

            string PoBox = string.Empty;

            // no po box  
            if (Regex.Match(AddressLine1, sPoBoxNoPOB).Length == 0
                && Regex.Match(AddressLine2, sPoBoxNoPOB).Length == 0)
            {
                return PoBox;
            }

            // po box length too long
            if (Regex.Match(AddressLine1, sPoBoxValidateMax).Length >= 18
                || Regex.Match(AddressLine2, sPoBoxValidateMax).Length >= 1)
            {
                throw new Exception("Invalid PO Box Number, too many characters");
            }

            // we have a po box but no number
            if (Regex.Match(AddressLine1, sPoBoxValidateMin).Length <= 7
                && Regex.Match(AddressLine2, sPoBoxValidateMin).Length <= 7)
            {
                throw new Exception("Invalid PO Box Number, too few characters");
            }

            // extract po box number from addr1
            if (Regex.IsMatch(AddressLine1, sPoBoxExtract))
            {
                PoBox = Regex.Match(AddressLine1, sPoBoxExtract).ToString();
                AddressLine1 = AddressLine1.Replace(PoBox, "").Trim();
                PoBox = Regex.Replace(PoBox, sPoBoxReplace, string.Empty);
                PoBox = PoBox.Trim();
            }
            // extract po box number from addr2
            if (Regex.IsMatch(AddressLine2, sPoBoxExtract))
            {
                PoBox = Regex.Match(AddressLine2, sPoBoxExtract).ToString();
                AddressLine2 = AddressLine2.Replace(PoBox, "").Trim();
                PoBox = Regex.Replace(PoBox, sPoBoxReplace, string.Empty);
                PoBox = PoBox.Trim();
            }

            return PoBox;
        }
        private static string ParseHouseNumber(string AddressLine1)
        {
            //regex patterns for house number 
            // valid house numbers can contain 1 to 10 characters (per SAP limit), beginning with a decimal, followed by decimal or alpha chars
            // may also end space and fraction 1/4 or 1/2

            string sHseNumValidate = "^[1-9][0-9a-zA-Z]{10}";
            string[] sHseNum = new string[]
            {
                "^[1-9][^\\s]{0,10}\\s",
                "^[1-9][^\\s]{0,6}\\s1/2",
                "^[1-9][^\\s]{0,6}\\s1/4"
            };

            int sPatternLength = 0;

            string houseNumber = string.Empty;


            AddressLine1 = AddressLine1 ?? AddressLine1.Replace("PO BOX", "").Trim();

            if (Regex.Match(AddressLine1, sHseNumValidate).Length > 10)
            {
                throw new Exception("Invalid House Number, too many characters");
            }

            // see if we can extract house number
            sPatternLength = sHseNum.Length;
            if (AddressLine1.Length < sHseNum.Length) sPatternLength = AddressLine1.Length - 1; // start with most complex and work towards least complex 

            for (int i = 0; i <= sPatternLength - 1; i++)  // loop through regex elements
            {
                if (Regex.IsMatch(AddressLine1, sHseNum[i]))
                {
                    houseNumber = Regex.Match(AddressLine1, sHseNum[i]).ToString();

                    // Before trimming the house number, take the result and remove it form the beginning of add1
                    AddressLine1 = AddressLine1.Substring(houseNumber.Length).Trim();
                    houseNumber = houseNumber.TrimEnd();
                    return houseNumber;
                }
            }

            return houseNumber;
        }

        private static string ParseStreet(string AddressLine1, string HouseNumber)
                                            => string.IsNullOrEmpty(AddressLine1) && string.IsNullOrEmpty(HouseNumber) ?
                                               string.Empty :
                                               AddressLine1.Substring(HouseNumber.Length).Trim();
        #endregion
    }
}
