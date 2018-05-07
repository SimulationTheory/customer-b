using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PSE.Customer.Tests.Unit.TestObjects;
using PSE.Customer.V1.Clients.Mcf.Models;
using PSE.Customer.V1.Logic.Extensions;
using PSE.RestUtility.Core.Mcf;
using Shouldly;

namespace PSE.Customer.Tests.Unit.V1.Logic.Extensions
{
    [TestClass]
    public class OwnerAccountsExtensionsTests
    {
        [TestMethod]
        public void ToModels_ResponseFromCustomerWithMultipleActiveAccounts_AccountsConvertedToModels()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetOwnerAccountMultiplePremises);
            var response = JsonConvert.DeserializeObject<McfResponse<McfResponseResults<OwnerAccountsSet>>>(testData);

            // Act
            var ownerAccountModels = response.Result.Results.ToModels();

            // Assert
            var ownerModelList = ownerAccountModels.ToList();
            ownerModelList.ShouldNotBeNull();
            ownerModelList.Count.ShouldBeGreaterThan(0);

            var firstAccount = ownerModelList[0];
            firstAccount.ServiceAddress.AddressLine1.ShouldBe("12401 111TH AVE E");
            firstAccount.ServiceAddress.City.ShouldBe("Puyallup");
            firstAccount.ServiceAddress.State.ShouldBe("WA");
            firstAccount.ServiceAddress.PostalCode.ShouldBe("98374");
            firstAccount.ContractAccountNumber.ShouldBe("200012482127");
            firstAccount.OccupiedStatus.ShouldBe("Occupied");
            firstAccount.StatusDate.ShouldBe("12/09/2013");

            var vacantAccount = ownerModelList.First(x => x.OccupiedStatus != "Occupied");
            vacantAccount.OccupiedStatus.ShouldBe("Vacant");
            vacantAccount.StatusDate.ShouldBe("07/01/2016");
        }
    }
}
