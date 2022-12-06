using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ApiRRHH.Web.Middleware
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationRoot GetConfiguration(this IWebHostEnvironment env)
        {
            return ConfigurationHelper.GetConfiguration(env.EnvironmentName, env.ContentRootPath);
        }

        public static IConfigurationRoot GetConfiguration(this IServiceCollection services)
        {
            return ConfigurationHelper.GetConfiguration(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        }
    }
}
