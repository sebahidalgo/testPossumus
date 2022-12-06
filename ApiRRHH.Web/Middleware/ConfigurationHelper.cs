using Microsoft.Extensions.Configuration;
using System;

namespace ApiRRHH.Web.Middleware
{
    public static class ConfigurationHelper
    {
        private static IConfigurationRoot _builtConfiguration;

        public static IConfigurationRoot GetConfiguration()
        {
            return GetConfiguration(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }

        private static IConfigurationBuilder GetConfigurationBuilder(string enviromentName, string contentRootPath = null)
        {
            var configurationbuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{enviromentName}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            if (contentRootPath != null)
            {
                configurationbuilder.SetBasePath(contentRootPath);
            }

            return configurationbuilder;
        }

        public static IConfigurationRoot GetConfiguration(string enviromentName, string contentRootPath = null)
        {
            if (_builtConfiguration != null)
                return _builtConfiguration;

            var configurationbuilder = GetConfigurationBuilder(enviromentName, contentRootPath);

            _builtConfiguration = configurationbuilder.Build();

            return _builtConfiguration;
        }
    }
}
