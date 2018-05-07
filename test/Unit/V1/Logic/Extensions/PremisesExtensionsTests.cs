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
    public class PremisesExtensionsTests
    {
        [TestMethod]
        public void ToModels_ResponseFromMultiplePremises_ConvertedToModels()
        {
            // Arrange
            var testData = TestData.GetFromResources(TestData.GetPremisesMultiplePremises);
            var response = JsonConvert.DeserializeObject<McfResponse<PremisesSet>>(testData);

            // Act
            var premiseModels = response.Result.ToModels();

            // Assert
            var premises = premiseModels.ToList();
            var firstPremise = premises[0];
            firstPremise.ServiceAddress.AddressLine1.ShouldBe("18116 SE 45TH ST");
            firstPremise.ServiceAddress.AddressLine2.ShouldBe("");
            firstPremise.ServiceAddress.City.ShouldBe("Issaquah");
            firstPremise.ServiceAddress.State.ShouldBe("WA");
            firstPremise.ServiceAddress.PostalCode.ShouldBe("98029");
            firstPremise.ContractAccountStartDate.ShouldBe("02/03/2018");
            firstPremise.ContractAccountEndDate.ShouldBe("05/03/2018");
        }
    }
}
