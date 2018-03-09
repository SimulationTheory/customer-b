using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.CertificateInstaller;
using PSE.Customer.Tests.Integration.TestObjects;

// ReSharper disable once CheckNamespace
namespace PSE.Customer.Extensions.Tests.Integration
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestClass]
        public class AddRespositories
        {

            [TestInitialize]
            public void Setup()
            {   
                //Waiting until certificates are installed
                Thread.Sleep(5000);
            }
        }
    }
}
