using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PSE.Cassandra.Core.Extensions;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.V1.Clients.Authentication;
using PSE.Customer.V1.Clients.Authentication.Interfaces;
using PSE.Customer.V1.Clients.ClientProxy;
using PSE.Customer.V1.Clients.ClientProxy.Interfaces;
using PSE.Customer.V1.Clients.Mcf;
using PSE.Customer.V1.Clients.Mcf.Interfaces;
using PSE.Customer.V1.Logic;
using PSE.Customer.V1.Logic.Interfaces;
using PSE.Customer.V1.Models;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.DefinedTypes;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Customer.V1.Response;
using PSE.WebAPI.Core.Startup;
using System;

namespace PSE.Customer.Extensions
{
    /// <summary>
    /// Extends IServiceCollection interface
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures and registers the statement and contract account repositories.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="logger"></param>
        public static IServiceCollection AddRepositories(this IServiceCollection services, ILogger logger)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Setup Cassandra
            var config = services.GetCoreOptions().Configuration;

            services.AddSingleton(Options.Create(config.CassandraSettings));
            services.AddCassandraConfiguration();
            services.AddCassandraMapping<MicroservicesKeyspace, AddressDefinedType>();
            services.AddCassandraMapping<MicroservicesKeyspace, PhoneDefinedType>();
            services.AddCassandraEntity<MicroservicesKeyspace, CustomerEntity>();
            services.AddCassandraEntity<MicroservicesKeyspace, CustomerContactEntity>();
            services.AddCassandraEntity<MicroservicesKeyspace, BPByContractAccountEntity>();

            // Setup repos and logic
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IBPByContractAccountRepository, BPByContractAccountRepository>();
            services.AddTransient<ICustomerLogic, CustomerLogic>();

            // Mapping Logic
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<GetCustomerProfileResponse, CustomerProfileModel>();
            });

            return services;
        }

        #region Private Methods

        /// <summary>
        /// Configures and registers the clients and services needed to call external services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The services</returns>
        public static IServiceCollection AddClientProxies(this IServiceCollection services)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient<IApiUser, ApiUser>();
            services.AddTransient<IClientProxy, ClientProxy>();
            services.AddTransient<IAuthenticationApi, AuthenticationApi>();
            services.AddTransient<IMcfClient, McfClient>();

            return services;
        }

        #endregion
    }
}