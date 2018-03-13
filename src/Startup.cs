using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PSE.Customer.Configuration;
using PSE.Customer.Extensions;
using PSE.WebAPI.Core.Startup;

namespace PSE.Customer
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        
        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="env">The env.</param>
        /// <exception cref="System.ArgumentNullException">
        /// logger
        /// or
        /// env
        /// </exception>
        public Startup(ILogger<Startup> logger, IHostingEnvironment env)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            env = env ?? throw new ArgumentNullException(nameof(env));

            _logger.LogInformation("Starting the account microservice.");

            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
                Configuration = builder.Build();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An exception occurred during Startup initialization.");

                throw;
            }
        }

        /// <summary>
        /// Configures the specified application.
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="provider">The provider.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));
            env = env ?? throw new ArgumentNullException(nameof(env));
            loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            provider = provider ?? throw new ArgumentNullException(nameof(provider));

            app.UseAuthentication();
            app.UsePSESwagger(provider, ServiceConfiguration.AppName);
            app.UseMvc();
        }

        /// <summary>
        /// Configures the PSE web api services. 
        /// </summary>
        /// <param name="services">The services.</param>
        // ReSharper disable once InconsistentNaming
        protected virtual void ConfigurePSEWebAPI(IServiceCollection services)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            services.ConfigurePSEWebAPI(ServiceConfiguration.AppName);
            services.InstallCassandraCertificates(_logger);
            services.InstallSecureMcfEndpointCertificate(_logger);
        }

        /// <summary>
        /// Configures the app services.
        /// Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public void ConfigureServices(IServiceCollection services)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            ConfigurePSEWebAPI(services);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddRepositories(_logger);
        }
    }
}