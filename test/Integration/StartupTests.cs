using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Customer.V1.Repositories.Views;
using PSE.Customer.Tests.Integration.TestObjects;
using PSE.Test.Core;

namespace PSE.Customer.Tests.Integration
{
    [TestClass]
    public class StartupTests
    {
        #region Test Helper Methods

        private static IApiVersionDescriptionProvider GetApiVersionDescriptionProvider(int majorVersion = 1, int minorVersion = 0, 
            string groupName = "TestGroupName", bool isDeprecated = false)
        {
            var provider = new Mock<IApiVersionDescriptionProvider>();
            provider.SetupAllProperties();
            provider.SetupGet(p => p.ApiVersionDescriptions)
                .Returns(() =>
                {
                    return new List<ApiVersionDescription>
                    {
                        new ApiVersionDescription(new ApiVersion(majorVersion, minorVersion), groupName, isDeprecated)
                    };
                });

            return provider.Object;
        }

        private static ApplicationBuilder GetApplicationBuilder()
        {
            var services = new ServiceCollection();
            services.AddTransient<MvcMarkerService>();
            services.AddTransient<MiddlewareFilterConfigurationProvider>();
            services.AddTransient<MiddlewareFilterBuilder>();
            services.AddTransient<IActionDescriptorCollectionProvider, ActionDescriptorCollectionProvider>();
            services.AddTransient<IActionInvokerFactory, ActionInvokerFactory>();
            services.AddTransient<IActionSelector, ActionSelector>();
            services.AddTransient<IEnumerable<IActionConstraintProvider>>(provider => new IActionConstraintProvider[]
                        {
                            new DefaultActionConstraintProvider()
                        });
            services.AddSingleton(new Mock<DiagnosticSource>().Object);
            services.AddTransient<MvcRouteHandler>();
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            services.AddSingleton(loggerFactory);
            services.AddTransient<ActionConstraintCache>();
            services.AddRouting();
            var serviceProvider = services.BuildServiceProvider();

            return new ApplicationBuilder(serviceProvider);
        }

        private static IHostingEnvironment GetHostingEnvironment(string environment)
        {
            return new HostingEnvironment()
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                EnvironmentName = environment
            };
        }

        private static Startup GetStartup()
        {
            var logger = TestHelper.GetLogger<Startup>();
            var env = GetHostingEnvironment(EnvironmentName.Development);

            return new Startup(logger, env);
        }

        #endregion Test Helper Methods


        [TestClass]
        public class Constructor
        {

            /// <summary>
            /// Tests the Startup(logger, env) constructor. Golden path
            /// </summary>
            [TestMethod]
            public void Startup_Test()
            {
                // init vars
                var env = GetHostingEnvironment(EnvironmentName.Development);
                var logger = TestHelper.GetLogger<Startup>();

                // test target constructor
                var result = new Startup(logger, env);

                // assertions
                Assert.IsNotNull(result.Configuration);
                Assert.IsNotNull(result.Configuration.GetSection("AppSettings"));
            }

            /// <summary>
            /// Tests the Startup(logger, env) constructor. Golden path
            /// </summary>
            [TestMethod]
            public void Startup_Production_Test()
            {
                // init vars
                var env = GetHostingEnvironment(EnvironmentName.Production);
                var logger = TestHelper.GetLogger<Startup>();

                // test target constructor
                var result = new Startup(logger, env);

                // assertions
                Assert.IsNotNull(result.Configuration);
            }
        }

        [TestClass]
        public class Configure
        {

            /// <summary>
            /// Tests the Configure() method to confirm the expected services are registered.
            /// </summary>
            [TestMethod]
            public void Configure_Test()
            {
                // init vars
                var app = GetApplicationBuilder();
                var env = GetHostingEnvironment(EnvironmentName.Development);
                var loggerFactory = new LoggerFactory();
                var provider = GetApiVersionDescriptionProvider();
                var target = GetStartup();

                // test target method
                target.Configure(app, env, loggerFactory, provider);
            }
        }

        [TestClass]
        public class ConfigureServices
        {

            /// <summary>
            /// Tests the ConfigureServices() method to confirm the expected services are registered.
            /// </summary>
            [TestMethod]
            public void ConfigureServices_E2E_Test()
            {
                // init vars
                //var connection = TestHelper.GetWebConfiguration().CassandraSettings.CreateCluster(null);
                var services = new ServiceCollection();
                services.AddSingleton<ILogger>(CoreHelper.GetLogger<Startup>());
                services.AddSingleton(TestHelper.GetSessionFacade());
                services.AddSingleton(CoreHelper.GetLogger<ContractAccountRepository>());
                services.AddSingleton(CoreHelper.GetLogger<CustomerLogic>());
                var target = GetStartup();

                var loggerFactory = new LoggerFactory();
                services.AddSingleton<ILoggerFactory>(loggerFactory);

                // test target method
                target.ConfigureServices(services);

                // assertions
                var provider = services.BuildServiceProvider();
                Assert.IsNotNull(provider.GetService<IEntity<ContractAccountEntity>>());
                Assert.IsNotNull(provider.GetService<IEntity<ContractAccountByBusinessPartnerView>>());
                Assert.IsNotNull(provider.GetService<IContractAccountRepository>());
                Assert.IsNotNull(provider.GetService<ICustomerLogic>());
            }
        }
    }
}
