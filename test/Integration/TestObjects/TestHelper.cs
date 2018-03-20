using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace PSE.Customer.Tests.Integration.TestObjects
{
    public static class TestHelper
    {
        /// <summary>
        /// Gets a new service collection with logging enabled and ApplicationLifetime registered.
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IApplicationLifetime, ApplicationLifetime>();
            Cassandra.Core.Extensions.ServiceCollectionExtensions.ClearCassandraSettings();
            return services;
        }
    }
}
