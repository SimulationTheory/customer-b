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
using PSE.Customer.Tests.Integration.TestObjects;

namespace PSE.Customer.Tests.Integration
{
    [TestClass]
    public class StartupE2ETests
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
            //var logger = TestHelper.GetLogger<Startup>();
            var env = GetHostingEnvironment(EnvironmentName.Development);

            return new Startup(null, env);
        }

        #endregion Test Helper Methods


        [TestClass]
        public class Constructor
        {

        }

        [TestClass]
        public class Configure
        {

        }

        [TestClass]
        public class ConfigureServices
        {
        }
    }
}
