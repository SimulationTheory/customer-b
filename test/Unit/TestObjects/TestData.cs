using System.IO;
using System.Reflection;

namespace PSE.Customer.Tests.Unit.TestObjects
{
    public static class TestData
    {
        public static string GetBpIdentifier = "PSE.Customer.Tests.Unit.TestObjects.Data.GetBpIdentifier.json";
        public static string GetBpIdentifierResults = "PSE.Customer.Tests.Unit.TestObjects.Data.GetBpIdentifierResults.json";
        public static string GetInstallmentPlan = "PSE.Customer.Tests.Unit.TestObjects.Data.GetInstallmentPlan.json";
        public static string GetOwnerAccountMultiplePremises = "PSE.Customer.Tests.Unit.TestObjects.Data.GetOwnerAccountMultiplePremises.json";
        public static string GetOwnerAccountNoActiveAccount = "PSE.Customer.Tests.Unit.TestObjects.Data.GetOwnerAccountNoActiveAccount.json";

        public static string GetFromResources(string resourceName)
        {
            var assem = Assembly.GetExecutingAssembly();
            using (var stream = assem.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
