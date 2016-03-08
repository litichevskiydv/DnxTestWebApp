namespace Web
{
    using System;
    using Autofac;
    using Domain;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Autofac.Extensions.DependencyInjection;
    using NLog.Extensions.Logging;
    using NLog.Web;
    using Domain.ValuesProvider;
    using JetBrains.Annotations;

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json");

            if (env.IsDevelopment())
                builder.AddApplicationInsightsSettings(true);

            builder.AddEnvironmentVariables();
            Configuration = builder
                .Build()
                .ReloadOnChanged("appsettings.json")
                .ReloadOnChanged($"appsettings.{env.EnvironmentName}.json");
        }

        private IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            services
                .AddOptions()
                .Configure<ValuesControllerConfiguration>(Configuration);

            services.AddMvc();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<SimpleValuesProvider>().As<IValuesProvider>().SingleInstance();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return container.Resolve<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            env.ConfigureNLog("nlog.config");

            app
                .UseIISPlatformHandler()
                .UseApplicationInsightsRequestTelemetry()
                .UseApplicationInsightsExceptionTelemetry();

            if (env.IsDevelopment())
                app
                    .UseDeveloperExceptionPage()
                    .UseRuntimeInfoPage()
                    .UseBrowserLink();
            else
                app.UseExceptionHandler("/Errors/Error500");

            app.UseStatusCodePagesWithReExecute("/Errors/Error{0}");

            app.UseMvc();
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
