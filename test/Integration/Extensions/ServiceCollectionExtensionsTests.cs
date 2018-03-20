using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSE.Cassandra.Core.Extensions;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.Configuration;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.Extensions;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Controllers;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Test.Core;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Startup;
using Shouldly;

namespace PSE.Customer.Tests.Integration.Extensions
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        [TestClass]
        public class AddRespositories
        {
#if DEBUG
            /// <summary>
            /// This test is intended to help catch situations where an interface is added without being added
            /// in AddRepositories or an associated method.  This results in a server error 500 at run time.
            /// </summary>
            /// <remarks>
            /// This test fails in the docker build environment, so its being ignored for release mode.
            /// Users should run locally before making a pull request.
            /// </remarks>
            [TestMethod]
#endif
            public void AddRepositories_E2E_CanInitializeRepositoryLogicAndControllerParams()
            {
                // Arrange
                var services = TestHelper.GetServiceCollection();
                var logger = CoreHelper.GetLogger<Startup>();
                services.ConfigurePSEWebAPI(ServiceConfiguration.AppName);

                using (services.GetLoggerFactory())
                {
                    // Act
                    var servicesReturned = services
                        .AddRepositories(logger)
                        .AddClientProxies();

                    // Assert
                    logger.ShouldNotBeNull();
                    servicesReturned.ShouldBe(services);
                    var provider = services.BuildServiceProvider();
                    provider.ShouldNotBeNull();
                    provider.GetService<ILogger>().ShouldNotBeNull();

                    // Repository
                    provider.GetService<ISessionFacade<MicroservicesKeyspace>>().ShouldNotBeNull();
                    provider.GetService<IEntity<CustomerEntity>>().ShouldNotBeNull();
                    provider.GetService<IEntity<CustomerContactEntity>>().ShouldNotBeNull();
                    provider.GetService<ILogger<CustomerRepository>>().ShouldNotBeNull();
                    provider.GetService<ICustomerRepository>().ShouldNotBeNull();

                    // Logic
                    provider.GetService<IDistributedCache>().ShouldNotBeNull();
                    provider.GetService<IMemoryCache>().ShouldNotBeNull();
                    provider.GetService<IOptions<AppSettings>>().ShouldNotBeNull();
                    provider.GetService<ILogger<CustomerLogic>>().ShouldNotBeNull();
                    provider.GetService<ICoreOptions>().ShouldNotBeNull();
                    provider.GetService<IBPByContractAccountRepository>().ShouldNotBeNull();
                    provider.GetService<ICustomerRepository>().ShouldNotBeNull();
                    provider.GetService<IAuthenticationApi>().ShouldNotBeNull();
                    provider.GetService<ICustomerLogic>().ShouldNotBeNull();

                    // Controller
                    provider.GetService<ILogger<CustomerController>>().ShouldNotBeNull();
                    provider.GetService<ICustomerLogic>().ShouldNotBeNull();
                }
            }
        }
    }
}
