using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.CertificateInstaller;
using PSE.Customer;
using PSE.Customer.Extensions;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Test.Core;

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
                var cluster = TestHelper.GetWebConfiguration().CassandraSettings;
                Installer.InstallRemoteCertificates(cluster.Hosts.Select(x => new Address()
                {
                    Host = x.IpAddress,
                    Name = x.HostName,
                    Port = cluster.Port
                }));
                //Waiting until certificates are installed
                Thread.Sleep(5000);
            }

            [TestMethod]
            public void AddRepositories_Test()
            {
                // init vars
                var services = new ServiceCollection();
                var logger = CoreHelper.GetLogger<Startup>();
                var loggerFactory = new LoggerFactory();
                services.AddSingleton<ILoggerFactory>(loggerFactory);

                // test target month
                services.AddRepositories(logger);
            }
        }
    }
}
