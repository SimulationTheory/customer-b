using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.Customer.Extensions;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Repositories.Entities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSE.Customer.Tests.Unit.Extensions
{
    [TestClass]
    public class CustomerExtensionsTests
    {
        #region Test Helper Methods

        private void TestToModelNullParameters(CustomerEntity source, string expectedParamName)
        {
            try
            {
                //Act
                source.ToModel();

                 //Assert
                Assert.Fail("The expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex);

                //Assert
                expectedParamName.ShouldBe(ex.ParamName);
            }
        }

        private void TestAddToModelNullParameters(CustomerContactEntity source, CustomerProfileModel model, string expectedParamName)
        {
            try
            {
                //Act
                source.AddToModel(model);

                //Assert
                Assert.Fail("The expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex);

                //Assert
                expectedParamName.ShouldBe(ex.ParamName);
            }
        }

        private void TestMcfToCassandraModelNullParameters(McfAddressinfo source, string expectedParamName)
        {
            try
            {
                //Act
                source.McfToCassandraModel();

                //Assert
                Assert.Fail("The expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex);

                //Assert
                expectedParamName.ShouldBe(ex.ParamName);
            }
        }

        private void TestCassandraToMcfModelNullParameters(AddressDefinedType source, string expectedParamName)
        {
            try
            {
                //Act
                source.CassandraToMcfModel();

                //Assert
                Assert.Fail("The expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex);

                //Assert
                expectedParamName.ShouldBe(ex.ParamName);
            }
        }

        #endregion Test Helper Methods

        #region ToModel Tests
        [TestMethod]
        public void ToModel_NullCustomerEntityThrows_Test()
        {
            // init vars
            const CustomerEntity source = null;
            const string expectedParamName = nameof(source);

            // test target month
            TestToModelNullParameters(source, expectedParamName);
        }

        [TestMethod]
        public void ToModel_FullName_Test()
        {
            // init vars
            var customerEntity = new CustomerEntity
            {
                BusinessPartnerId = 1002647070,
                FullName = "JENNIFER L POWERS",
                FirstName = "JENNIFER",
                LastName = "POWERS",
                EmployerName = "Puget Sound Energy",
                PvaIndicator = true
            };

            var model = customerEntity.ToModel();

            model.ShouldNotBeNull();
            model.ShouldBeOfType<CustomerProfileModel>();
            model.CustomerName.ShouldBe(customerEntity.FullName);
            model.OrganizationName.ShouldBe(customerEntity.EmployerName);
            model.IsPva.ShouldBe(customerEntity.PvaIndicator);
        }

        [TestMethod]
        public void ToModel_NoFullName_Test()
        {
            // init vars
            var customerEntity = new CustomerEntity
            {
                BusinessPartnerId = 1002647070,
                FullName = null,
                FirstName = "JENNIFER",
                LastName = "POWERS",
                EmployerName = "Puget Sound Energy",
                PvaIndicator = true
            };

            var model = customerEntity.ToModel();

            model.ShouldNotBeNull();
            model.ShouldBeOfType<CustomerProfileModel>();
            model.CustomerName.ShouldContain(customerEntity.FirstName);
            model.CustomerName.ShouldContain(customerEntity.LastName);
            model.OrganizationName.ShouldBe(customerEntity.EmployerName);
            model.IsPva.ShouldBe(customerEntity.PvaIndicator);
        }
        #endregion

        #region AddToModel Tests
        [TestMethod]
        public void AddToModel_NullCustomerContactEntityThrows_Test()
        {
            // init vars
            const CustomerContactEntity source = null;
            CustomerProfileModel model = new CustomerProfileModel();
            const string expectedParamName = nameof(source);

            // test target month
            TestAddToModelNullParameters(source, model, expectedParamName);
        }

        [TestMethod]
        public void AddToModel_NullCustomerProfileModelThrows_Test()
        {
            // init vars
            var source = new CustomerContactEntity();
            CustomerProfileModel model = null;
            const string expectedParamName = nameof(model);

            // test target month
            TestAddToModelNullParameters(source, model, expectedParamName);
        }

        [TestMethod]
        public void AddToModel_Test()
        {
            //Arrange
            var user = TestHelper.PaDev1;
            var customerContactEntity = new CustomerContactEntity
            {

                Email = user.Email,
                MailingAddress = user.Address,
                Phones = new Dictionary<string, PhoneDefinedType>
                        {
                            {
                                "cell", new PhoneDefinedType
                                {
                                    Number = user.Phones[0].Number,
                                    Extension = user.Phones[0].Extension
                                }
                            },
                            {
                                "work", new PhoneDefinedType
                                {
                                    Number = user.Phones[1].Number,
                                    Extension = user.Phones[1].Extension
                                }
                            }
                        }
            };
            CustomerProfileModel model = new CustomerProfileModel();

            // Act
            customerContactEntity.AddToModel(model);
            model.EmailAddress.ShouldBe(customerContactEntity.Email);
            model.MailingAddress.ShouldBe(customerContactEntity.MailingAddress);

            var firstPhone = model.Phones.FirstOrDefault();
            firstPhone.Number.ShouldBe(customerContactEntity.Phones["cell"].Number);
            firstPhone.Extension.ShouldBe(customerContactEntity.Phones["cell"].Extension);

            var lastPhone = model.Phones.Last();
            lastPhone.Number.ShouldBe(customerContactEntity.Phones["work"].Number);
            lastPhone.Extension.ShouldBe(customerContactEntity.Phones["work"].Extension);
        }
        #endregion

        #region McfToCassandraModel Tests
        [TestMethod]
        public void McfToCassandraModel_NullMcfAddressinfoThrows_Test()
        {
           //Arrange
            const McfAddressinfo source = null;

            const string expectedParamName = nameof(source);

            // Act & Assert
            TestMcfToCassandraModelNullParameters(source, expectedParamName);
        }
        [TestMethod]
        public void McfToCassandraModel_Test()
        {
            // Arrange
            var addressInfo = new McfAddressinfo
                        {
                           Street = "SE 166TH ST",
                           HouseNo = "10502",
                           City = "Renton",
                           PostalCode = "98055",
                           Region = "WA",
                           CountryID = "US",
                           HouseNo2 = "Apt H3"
                          };

            var model = addressInfo.McfToCassandraModel();

            model.ShouldNotBeNull();
            model.ShouldBeOfType<AddressDefinedType>();
            model.AddressLine1.ShouldBe($"{addressInfo.HouseNo?.Trim()} {addressInfo.Street.Trim()}");
            model.AddressLine2.ShouldBe(addressInfo.HouseNo2);
            model.City.ShouldBe(addressInfo.City);
            model.PostalCode.ShouldBe(addressInfo.PostalCode);
            model.State.ShouldBe(addressInfo.Region);
            model.Country.ShouldBe(addressInfo.CountryID);
        }
        #endregion



        #region CassandraToMcfModel Tests
        [TestMethod]
        public void CassandraToMcfModel_NullMcfAddressinfoThrows_Test()
        {
            //Arrange
            const AddressDefinedType source = null;

            const string expectedParamName = nameof(source);

            // Act & Assert
            TestCassandraToMcfModelNullParameters(source, expectedParamName);
        }
        [TestMethod]
        public void CassandraToMcfModel_Test()
        {
            // Arrange
            var address = new AddressDefinedType
            {
                AddressLine1 = " 10502 SE 166TH ST",
                AddressLine2 = "Apt H3  ",
                City = " Renton ",
                PostalCode = "98055",
                State = "WA",
                Country = "US"
            };

            var model = address.CassandraToMcfModel();

            model.ShouldNotBeNull();
            model.ShouldBeOfType<McfAddressinfo>();
            model.POBox.ShouldBe(address.AddressLine1.Trim());
            model.Street.ShouldBe(address.AddressLine1.Trim());
            model.HouseNo.ShouldBe(address.AddressLine1.Trim());
            model.HouseNo2.ShouldBe(address.AddressLine2.Trim());
            model.PostalCode.ShouldBe(address.PostalCode.Trim());
            model.POBoxPostalCode.ShouldBe(address.PostalCode.Trim());
            model.City.ShouldBe(address.City.Trim());
            model.Region.ShouldBe(address.State.Trim());
            model.CountryID.ShouldBe(address.Country.Trim());
        }
        #endregion
    }
}
