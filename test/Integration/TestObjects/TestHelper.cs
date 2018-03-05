using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PSE.Cassandra.Core.Extensions;
using PSE.Cassandra.Core.Session;
using PSE.Cassandra.Core.Session.Interfaces;
using PSE.Customer.Configuration;
using PSE.Customer.Configuration.Keyspaces;
using PSE.Customer.V1.Repositories;
using PSE.Customer.V1.Repositories.Entities;
using PSE.Customer.V1.Repositories.Interfaces;
using PSE.Customer.V1.Repositories.Views;
using PSE.Test.Core;
using PSE.WebAPI.Core.Configuration;

namespace PSE.Customer.Tests.Integration.TestObjects
{
    public static class TestHelper
    {
        public static readonly TestUser TestUser1 = new TestUser
        {
            ContractAccountId = 220006073096,
            BPNumber = 1002736444,
            LastPasswordChanged = "12/20/2017 9:26:59 PM",
            Username = "RESIMPSON"
        };

        public static readonly TestUser TestUser2 = new TestUser
        {
            ContractAccountId = 200027879325,
            BPNumber = 1001694236,
            LastPasswordChanged = "12/20/2017 9:30:44 PM",
            Username = "KEVAMHEARN"
        };

        public static readonly TestUser TestUser3 = new TestUser
        {
            ContractAccountId = 200010619514,
            BPNumber = 1001361917,
            LastPasswordChanged = "12/20/2017 9:34:13 PM",
            Username = "DIAMONDSQUARELLC"
        };

        public static readonly TestUser TestUser4 = new TestUser
        {
            ContractAccountId = 200014665042,
            BPNumber = 1002505700,
            LastPasswordChanged = "1/6/2018 2:50:25 AM",
            Username = "GlennSJohnson"
        };

        public static readonly string TestNotificationId = "37b78e90-814b-4ff9-8aa1-4c0422fd8ae1";

        /// <summary>
        /// Gets the application settings stored in appSettings.json.
        /// </summary>
        /// <returns></returns>
        public static AppSettings GetAppSettings()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables();
            var configuration = builder.Build();
            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);

            return appSettings;
        }

        public static IContractAccountRepository GetContractAccountRepository()
        {
            var accountSession = GetSession<ContractAccountEntity>();
            var bpSession = GetSession<ContractAccountByBusinessPartnerView>();
            var logger = GetLogger<ContractAccountRepository>();

            return new ContractAccountRepository(accountSession, bpSession, logger);
        }

        /// <summary>
        /// Gets the console logger.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger<T> GetLogger<T>()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Trace, true);

            return loggerFactory.CreateLogger<T>();
        }

        public static IEntity<TEntity> GetSession<TEntity>()
        {
            // register cassandra session used by the repos
            var config = GetWebConfiguration();
            var logger = CoreHelper.GetLogger<MicroservicesKeyspace>();
            var cluster = config.CassandraSettings.CreateCluster(logger);
            var sessionFacade = new SessionFacade<MicroservicesKeyspace>(cluster, logger);

            return new Entity<MicroservicesKeyspace, TEntity>(sessionFacade, logger);
        }

        public static ISessionFacade<MicroservicesKeyspace> GetSessionFacade()
        {
            return GetSessionFacade<MicroservicesKeyspace>();
        }

        public static ISessionFacade<TKeyspace> GetSessionFacade<TKeyspace>()
            where TKeyspace : Keyspace, new()
        {
            var config = GetWebConfiguration();
            var logger = CoreHelper.GetLogger<SessionFacade<TKeyspace>>();
            var cluster = config.CassandraSettings.CreateCluster(logger);

            return new SessionFacade<TKeyspace>(cluster, logger);
        }

        /// <summary>
        /// Gets the local web configuration.
        /// </summary>
        /// <returns></returns>
        public static WebConfiguration GetWebConfiguration()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("localConfiguration.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables();
            var configuration = builder.Build();
            var webConfig = new WebConfiguration();
            configuration.Bind(webConfig);

            return webConfig;
        }
    }
}
