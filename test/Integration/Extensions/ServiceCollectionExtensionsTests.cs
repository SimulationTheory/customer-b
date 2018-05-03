using Microsoft.AspNetCore.Http;
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
using PSE.Customer.V1.Clients.Address.Interfaces;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Controllers;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Test.Core;
using PSE.WebAPI.Core.Configuration.Interfaces;
using PSE.WebAPI.Core.Service;
using PSE.WebAPI.Core.Service.Interfaces;
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
            [Ignore]
#endif
            public void AddRepositories_E2E_CanInitializeRepositoryLogicAndControllerParams()
            {
                // Arrange
                var services = TestHelper.GetServiceCollection();
                AutoMapper.Mapper.Reset();

                using (var loggerFactory = services.GetLoggerFactory())
                {
                    var logger = loggerFactory.CreateLogger<Startup>();
                    TestHelper.GetCoreOptions();
                    services.AddClientProxies()
                        .AddTransient<CustomerController>()
                        .AddTransient<ManagePremisesController>()
                        .AddTransient<MoveInController>()
                        .AddTransient<MoveOutController>()
                        .AddScoped<IRequestContextAdapter, RequestContextAdapter>()
                        .AddScoped<IHttpContextAccessor, HttpContextAccessor>()
                        .AddSingleton(CoreHelper.GetMemoryDistributedCache())
                        .AddSingleton(CoreHelper.GetMemoryCache())
                        .AddScoped<IRequestContextAdapter, TestRequestContextAdapter>();

                    // Act
                    var servicesReturned = services.AddRepositories(logger);

                    // Assert
                    servicesReturned.ShouldBe(services);
                    var provider = services.BuildServiceProvider();
                    provider.GetRequiredService<ILogger>();

                    // Repository
                    provider.GetRequiredService<ISessionFacade<MicroservicesKeyspace>>();
                    provider.GetRequiredService<IEntity<CustomerEntity>>();
                    provider.GetRequiredService<IEntity<CustomerContactEntity>>();
                    provider.GetRequiredService<ILogger<CustomerRepository>>();
                    provider.GetRequiredService<ICustomerRepository>();
                    provider.GetRequiredService<IEntity<BPByContractAccountEntity>>();
                    provider.GetRequiredService<ILogger<BPByContractAccountRepository>>();
                    provider.GetRequiredService<IBPByContractAccountRepository>();

                    // Logic
                    provider.GetRequiredService<IDistributedCache>();
                    provider.GetRequiredService<IMemoryCache>();
                    provider.GetRequiredService<IOptions<AppSettings>>();
                    provider.GetRequiredService<ILogger<CustomerLogic>>();
                    provider.GetRequiredService<ICoreOptions>();
                    provider.GetRequiredService<IAuthenticationApi>();
                    provider.GetRequiredService<IMcfClient>();
                    provider.GetRequiredService<ICustomerLogic>();
                    provider.GetRequiredService<IAddressApi>();
                    provider.GetRequiredService<IManagePremisesLogic>();

                    // Controller
                    provider.GetRequiredService<CustomerController>();
                    provider.GetRequiredService<ManagePremisesController>();
                    provider.GetRequiredService<MoveInController>();
                    provider.GetRequiredService<MoveOutController>();
                }
            }
        }
    }
}
