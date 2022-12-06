using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace ApiRRHH.Web.Middleware
{
    public static class Extensions
    {
        public static LoggerConfiguration WithApplicationName(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich == null)
                throw new ArgumentNullException(nameof(enrich));

            return enrich.With<ApplicationNameEnricher>();
        }

        public static IServiceCollection AddActivityLog(this IServiceCollection service)
        {
            service.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return service;
        }

        public static IWebHostBuilder UseLogging(this IWebHostBuilder builder)
        {
            Serilog.Debugging.SelfLog.Enable(Console.Out);

            builder.ConfigureLogging((hostingContext, logBuilder) =>
            {
                logBuilder.ClearProviders();

                var configuration = hostingContext.HostingEnvironment.GetConfiguration();

                var log = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.WithThreadId()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.FromLogContext()
                    .Enrich.WithApplicationName()
                    .CreateLogger();

                Log.Logger = log;
            });

            builder.UseSerilog();

            return builder;
        }

        public static void UseActivityLog(this IApplicationBuilder app)
        {
            app.UseMiddleware<ActivityLogMiddleware>();
        }

        
    }
}
