using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PSE.Customer.Tests.Unit.TestObjects;
using PSE.Customer.V1.Clients.Extensions;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Clients.Mcf.Response;
using PSE.RestUtility.Core.Mcf;
using Shouldly;

namespace PSE.Customer.Tests.Unit.V1.Clients
{
    [TestClass]
    public class McfClientTests
    {
        #region BP Identifier Types Tests

        [TestMethod]
        public void DeserializeObject_GetBpIdentifierResults_CanParseResponseData()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetBpIdentifierResults);

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<BpIdentifier>>>(testData);

            // Assert
            var bpIds = response.Result.Results.ToList();
            bpIds.Count.ShouldBe(3);

            bpIds[0].AccountId.ShouldBe("1000001179");
            bpIds[0].IdentifierType.ShouldBe("ZDOB");
            bpIds[0].IdentifierNo.ShouldBe("02/02/1978");
            bpIds[0].IdValidFromDate.ShouldBe(null);
            bpIds[0].IdValidToDate.ShouldBe(null);

            bpIds[1].AccountId.ShouldBe("1000001179");
            bpIds[1].IdentifierType.ShouldBe("ZLAST4");
            bpIds[1].IdentifierNo.ShouldBe("1010");

            bpIds[2].AccountId.ShouldBe("1000001179");
            bpIds[2].IdentifierType.ShouldBe("ZDNAC");
            bpIds[2].IdentifierNo.ShouldBe("X");
            bpIds[2].IdValidFromDate.ShouldBe("/Date(1518480000000)/");
            bpIds[2].IdValidToDate.ShouldBe("/Date(253402214400000)/");
        }

        [TestMethod]
        public void DeserializeObject_GetBpIdentifier_CanParseResponseData()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetBpIdentifier);

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<BpIdentifier>>(testData);

            // Assert
            var bpId = response.Result;
            bpId.AccountId.ShouldBe("1000001179");
            bpId.IdentifierType.ShouldBe("ZDNAC");
            bpId.IdEntryDate.ShouldBe("/Date(1518270000000)/");
            bpId.IdentifierNo.ShouldBe("X");
            bpId.IdValidFromDate.ShouldBe("/Date(1518480000000)/");
            bpId.IdValidToDate.ShouldBe("/Date(253402214400000)/");
            bpId.Country.ShouldBe("UNITED STATES");
            bpId.CountryIso.ShouldBe("USA");
            bpId.Region.ShouldBe("004");
        }

        #endregion

        #region GetPaymentArrangement Tests

        [TestMethod]
        public void GetPaymentArrangement_AccountWithArrangement_CanParseResponseData()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetInstallmentPlan);

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<PaymentArrangementResponse>>(testData);

            // Assert
            response.Result.ShouldBeOfType<PaymentArrangementResponse>();
            response.Result.Results.Count.ShouldBeGreaterThanOrEqualTo(1);

            var paResult = response.Result.Results[0];
            paResult.ContractAccountID.ShouldBe("200019410436");
            paResult.NoOfPaymentsPermitted.ShouldBe("000");
            paResult.PaStatus.ShouldBe("D");
            paResult.InstallmentPlanNumber.ShouldBe("");
            paResult.InstallmentPlanType.ShouldBe("");
            paResult.PaymentArrangementNav.Results.Count.ShouldBeGreaterThan(0);

            var arrangement = paResult.PaymentArrangementNav.Results[0];
            arrangement.InstallmentPlanNumber.ShouldBe("400000002557");
            arrangement.InstallmentPlanType.ShouldBe("05");
            arrangement.NoOfInstallments.ShouldBe("002");
            arrangement.InstallmentPlansNav.Results.Count.ShouldBeGreaterThan(0);

            // First payment is payed so amount open is zero.  Second payment, unpaid amount is amount due.
            var firstPayment = arrangement.InstallmentPlansNav.Results[0];
            firstPayment.AmountDue.ShouldBe(500m);
            firstPayment.AmountOpen.ShouldBe(0);
            firstPayment.DueDate.ShouldBe("/Date(1518652800000)/");

            firstPayment.DueDate.Between("(", ")").ShouldBe("1518652800000");
            var dueDate = firstPayment.DueDate.Between("(", ")").FromUnixTimeSeconds();
            dueDate.ShouldNotBeNull();
            dueDate.Value.ShouldBe(new DateTime(2018, 2, 15, 0, 0, 0));

            var secondPayment = arrangement.InstallmentPlansNav.Results[1];
            secondPayment.AmountDue.ShouldBe(228.51m);
            secondPayment.AmountOpen.ShouldBe(228.51m);
            secondPayment.DueDate.ShouldBe("/Date(1519862400000)/");
        }

        #endregion

        #region GetPremises Tests

        [TestMethod]
        public void GetPremises_ResponseFromCustomerWithMultipleActiveAccounts_AccountsLoaded()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetPremisesMultiplePremises);

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<PremisesSet>>(testData);

            // Assert
            response.Error.ShouldBeNull();

            var premiseSet = response.Result;
            premiseSet.PremiseId.ShouldBe("7000012644");
            premiseSet.PremiseTypeId.ShouldBe("");

            var address = premiseSet.Address;
            address.Standardaddress.ShouldBeFalse();
            address.City.ShouldBe("Issaquah");
            address.PostlCod1.ShouldBe("98029");
            address.PoBox.ShouldBe("");
            address.Street.ShouldBe("SE 45TH ST");
            address.HouseNo.ShouldBe("18116");
            address.Region.ShouldBe("WA");

            var installations = premiseSet.Installations.Results.ToList();
            installations.Count.ShouldBe(2);
            var firstInstall = installations[0];
            firstInstall.MoveInEligibility.ShouldBe("X");
            firstInstall.InstallationId.ShouldBe("5000014928");
            firstInstall.DivisionId.ShouldBe("10");
            firstInstall.PremiseId.ShouldBe("7000012644");
            firstInstall.MoveInDateFrom.ShouldNotBeNull();
            firstInstall.MoveInDateFrom.Value.Date.ShouldBe(new DateTime(2018, 2, 3));
            firstInstall.MoveInDateTo.ShouldNotBeNull();
            firstInstall.MoveInDateTo.Value.Date.ShouldBe(new DateTime(2018, 5, 3));
            firstInstall.FutureMoveInDate.ShouldBeNull();
        }

        [TestMethod]
        public void GetPremises_ResponseFromCustomerWithNoActiveAccount_ErrorMessageReturned()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetPremisesNoActiveAccount);

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<PremisesSet>>>(testData);

            // Assert
            response.Error.ShouldNotBeNull();
            response.Error.Message.Value.ShouldContain("Record 1201162132");
            response.Error.Message.Value.ShouldContain("not found in table EVBS");
        }

        #endregion

        #region GetOwnerAccounts Tests

        [TestMethod]
        public void GetOwnerAccounts_ResponseFromCustomerWithMultipleActiveAccounts_AccountsLoaded()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetOwnerAccountMultiplePremises);

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<OwnerAccountsSet>>>(testData);

            // Assert
            response.Error.ShouldBeNull();
            var ownerAccountsSets = response.Result.Results.ToList();
            ownerAccountsSets.Count.ShouldBe(1);

            var ownerAccount = ownerAccountsSets[0];
            ownerAccount.AccountId.ShouldBe("1001543028");

            var contractAccounts = ownerAccount.OwnerContractAccount.Results.ToList();
            contractAccounts.Count.ShouldBeGreaterThan(0);

            var firstAccount = contractAccounts[0];
            firstAccount.ContractAccount.ShouldBe("200012482127");
            firstAccount.ContractAccountBalance.ShouldBe("0.000");

            var premiseSets = firstAccount.OwnerPremise.Results.ToList();
            premiseSets.Count.ShouldBe(1);

            var premise = premiseSets[0];
            premise.Premise.ShouldBe("7001424713");

            var premiseAddress = premise.PremiseAddress;
            premiseAddress.Standardaddress.ShouldBeFalse();
            premiseAddress.City.ShouldBe("Puyallup");
            premiseAddress.PostlCod1.ShouldBe("98374");
            premiseAddress.Street.ShouldBe("111TH AVE E");
            premiseAddress.HouseNo.ShouldBe("12401");

            var propertySets = premise.OwnerPremiseProperty.Results.ToList();
            propertySets.Count.ShouldBe(2);

            var firstProperty = propertySets[0];
            firstProperty.Property.ShouldBe("1300034560");
            firstProperty.Installation.ShouldBe("5000001007");
            firstProperty.Division.ShouldBe("20");
            firstProperty.Occupiedstatus.ShouldBe("Occupied");

            // ReSharper disable PossibleInvalidOperationException
            firstProperty.Opendate.Value.ToString("d", CultureInfo.InvariantCulture).ShouldBe("03/30/2013");
            firstProperty.Closedate.Value.ToString("d", CultureInfo.InvariantCulture).ShouldBe("12/31/9999");
            firstProperty.Lastoccupied.ShouldBeNull();
            firstProperty.Occupiedsince.Value.ToString("d", CultureInfo.InvariantCulture).ShouldBe("12/09/2013");
            // ReSharper restore PossibleInvalidOperationException
        }

        [TestMethod]
        public void GetOwnerAccounts_ResponseFromCustomerWithNoActiveAccount_ErrorMessageReturned()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetOwnerAccountNoActiveAccount);

            // Act
            var response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<BpIdentifier>>>(testData);

            // Assert
            response.Error.ShouldNotBeNull();
            response.Error.Message.Value.ShouldStartWith("No active properties exist for the BusinessPartner");
        }

        #endregion
    }
}
