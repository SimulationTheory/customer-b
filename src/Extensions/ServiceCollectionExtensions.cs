using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PSE.Cassandra.Core.Extensions;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Startup;

namespace PSE.Customer.Extensions
{
    public static class ServiceCollectionExtensions
    {


        /// <summary>
        /// Configures and registers the statement and contract account repositories.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="logger"></param>
        public static void AddRepositories(this IServiceCollection services, ILogger logger)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Setup Cassandra
            var config = services.GetCoreOptions().Configuration;
            services.AddCassandraConfiguration(config.CassandraSettings, services.GetLoggerFactory());
            services.AddCassandraMapping<MicroservicesKeyspace, AddressDefinedType>();
            services.AddCassandraMapping<MicroservicesKeyspace, PhoneDefinedType>();
            services.AddCassandraEntity<MicroservicesKeyspace, CustomerEntity>();
            services.AddCassandraEntity<MicroservicesKeyspace, CustomerContactEntity>();

            // Setup repos and logic
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ICustomerLogic, CustomerLogic>(); 
            
            //Mapping Logic

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<GetCustomerProfileResponse, CustomerProfileModel>();
            });
        }


        #region Private Methods
        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static ILoggerFactory GetLoggerFactory(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            return provider.GetService<ILoggerFactory>();
        }
        #endregion
    }
}