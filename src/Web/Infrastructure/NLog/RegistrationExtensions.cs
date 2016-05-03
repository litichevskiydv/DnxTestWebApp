namespace Web.Infrastructure.NLog
{
    using System.IO;
    using System.Reflection;
    using global::NLog;
    using global::NLog.Config;
    using global::NLog.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public static class RegistrationExtensions
    {
        private const string NLogConfigurationPathKey = "NLOG_CONFIGURATION_PATH";

        public static IConfigurationBuilder AddNLogConfig(this IConfigurationBuilder configurationBuilder, string configFileRelativePath)
        {
            var fullPathToConfigFile = Path.Combine(configurationBuilder.GetBasePath(), configFileRelativePath);
            var provider = new NLogConfigurationProvider(NLogConfigurationPathKey, fullPathToConfigFile);
            configurationBuilder.Add(provider);
            return configurationBuilder;
        }

        public static ILoggerFactory AddNLog(this ILoggerFactory factory, IConfigurationRoot configuration)
        {
            LogManager.AddHiddenAssembly(typeof (AspNetExtensions).GetTypeInfo().Assembly);
            factory.AddProvider(new NLogLoggerProvider());
            LogManager.Configuration = new XmlLoggingConfiguration(configuration[NLogConfigurationPathKey], true);
            return factory;
        }

        private class NLogConfigurationProvider : ConfigurationProvider
        {
            internal NLogConfigurationProvider(string key, string path)
            {
                Data[key] = path;
            }
        }
    }
}