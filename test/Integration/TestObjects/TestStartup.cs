using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PSE.Customer.Configuration;
using PSE.Test.Core.Auth;
using PSE.WebAPI.Core.Startup;

namespace PSE.Customer.Tests.Integration.TestObjects
{
    public class TestStartup : Startup
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public TestStartup(ILogger<Startup> logger, IHostingEnvironment env) : base(logger, env)
        {
            _logger = logger;

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            _loggerFactory = new LoggerFactory().AddConsole(LogLevel.Trace);
        }

        protected override void ConfigurePSEWebAPI(IServiceCollection services)
        {
            // replace loggerfactory
            services.Replace(ServiceDescriptor.Singleton(_loggerFactory));

            // DO NOT REMOVE - needed to wire up mvc. 
            services.ConfigurePSEWebAPI(ServiceConfiguration.AppName);
        }
    }
}
