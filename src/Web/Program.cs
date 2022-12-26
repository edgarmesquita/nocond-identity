using System;
using System.Collections.Generic;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace NoCond.Identity.Web
{
    public class Program
    {
        private static IConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        protected Program () { }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder (string[] args) =>
            Host.CreateDefaultBuilder (args)
            .UseLamar ()
            .ConfigureWebHostDefaults (webBuilder =>
            {
                webBuilder

                    .UseSerilog ((webHostBuilderCtx, loggerConfig) =>
                    {
                        config = webHostBuilderCtx.Configuration;

                        loggerConfig
                            .MinimumLevel.Debug ()
                            .MinimumLevel.Override ("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override ("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                            .MinimumLevel.Override ("System", LogEventLevel.Warning)
                            .MinimumLevel.Override ("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                            .MinimumLevel.Override ("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                            .MinimumLevel.Override ("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Debug)
                            .ReadFrom.Configuration (webHostBuilderCtx.Configuration)
                            .Enrich.FromLogContext ();
                    })
                    .ConfigureLogging ((webHostBuilderContext, loggingBuilder) => loggingBuilder.AddConsole ())
                    .UseKestrel(o =>
                    {
                        o.Limits.MaxRequestLineSize = 8 * 2048;
                        o.Limits.MaxRequestHeadersTotalSize = 32 * 2048;
                    })
                    .UseStartup<Startup> ();
                webBuilder.ConfigureServices (services =>
                {
                    // This is important, the call to AddControllers()
                    // cannot be made before the usage of ConfigureWebHostDefaults
                    services.AddControllers ();
                });
            })
            .ConfigureAppConfiguration ((hostContext, builder) =>
            {
                if (hostContext.HostingEnvironment.IsDevelopment ())
                {
                    builder.AddUserSecrets<Program> ();
                }
            });

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main (string[] args)
        {
            config = new ConfigurationRoot (new List<IConfigurationProvider> ());

            try
            {
                CreateHostBuilder (args)
                    .Build ()
                    .Run ();
            }
            catch (Exception e)
            {
                Log.Logger = new LoggerConfiguration ()
                    .ReadFrom.Configuration (config)
                    .CreateLogger ();
                Log.Fatal ("Host terminated unexpectedly -- Exception: {@e}", e);
            }
            finally
            {
                Log.CloseAndFlush ();
            }
        }
    }
}