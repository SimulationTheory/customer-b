using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PSE.Customer.Configuration;
using PSE.Test.Core.Auth;
using PSE.WebAPI.Core.Startup;

namespace PSE.Customer.Tests.Integration.TestObjects
{
    public class TestStartup : Startup
    {
        private readonly ILogger _logger;

        public TestStartup(ILogger<Startup> logger, IHostingEnvironment env) : base(logger, env)
        {
            _logger = logger;
        }

        protected override void ConfigurePSEWebAPI(IServiceCollection services)
        {
            //var config = TestHelper.GetWebConfiguration();
            //// install the certificates
            //var certAdddresses = config.CassandraSettings.Hosts.Select(x =>
            //    new Address()
            //    {
            //        Port = config.CassandraSettings.Port,
            //        Host = x.IpAddress
            //    });


            //Installer.InstallRemoteCertificates(certAdddresses);
            //services.AddCassandraConfiguration(config.CassandraSettings, services.GetLoggerFactory());
            //services.AddCassandraMapping<MicroservicesKeyspace, AddressDefinedType>();
            //services.AddCassandraEntity<MicroservicesKeyspace, ContractAccountEntity>();
            //services.AddCassandraEntity<MicroservicesKeyspace, ContractAccountByBusinessPartnerView>();
            //services.AddCassandraEntity<MicroservicesKeyspace, NotificationHistoryEntity>();

            // Add Test Auth
            services.ConfigureTestPSEWebAPI();

            // config
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            //appSettings.AuthenticationProvider = null; // skip auth config

            // DO NOT REMOVE - needed to wire up mvc. 
            // Auth is bypassed because integ/localConfiguration.json.AuthProviders=null
            services.ConfigurePSEWebAPI(ServiceConfiguration.AppName);
        }
    }
}
