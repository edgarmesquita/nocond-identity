using System;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NoCond.Identity.Application;
using NoCond.Identity.Application.Extensions;
using NoCond.Identity.Application.Settings;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Infrastructure.Extensions;
using NoCond.Identity.Persistence;
using NoCond.Identity.Persistence.Extensions;
using NoCond.Identity.Persistence.Stores;
using NoCond.Identity.Web.Extensions;
using Serilog;

namespace NoCond.Identity.Web
{
    public class Startup
    {
        private const int MaxSqlRetries = 5;
        private readonly string _corsPolicyName;
        private readonly string _spaSourcePath;
        private readonly ILogger logger;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            Configuration = config;
            Env = env;
            _spaSourcePath = Configuration.GetValue<string>("SPA:SourcePath");
            _corsPolicyName = Configuration.GetValue<string>("CORS:PolicyName");

            // Serilog logger can't be used at this point because Serilog hasn't been set yet
            logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger().ForContext<Startup>();

            logger.Information(
                $"Application ConfigMap{Environment.NewLine}{string.Join(Environment.NewLine, config.AsEnumerable())}");
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        /// <summary>
        ///     Configures the container.
        /// </summary>
        /// <remarks>
        ///     Take in Lamar's ServiceRegistry instead of IServiceCollection
        ///     as your argument, but fear not, it implements IServiceCollection
        ///     as well
        /// </remarks>
        /// <param name="services">The services.</param>
        public void ConfigureContainer(ServiceRegistry services)
        {
            services
                .IncludeWebServiceRegistry()
                .IncludeApplicationServiceRegistry()
                .IncludeInfrastructureServiceRegistry()
                .IncludePersistenceServiceRegistry();

            if (Env.IsDevelopment())
            {
                var container = new Container(services);
                logger.Information(container.WhatDidIScan());
                logger.Information(container.WhatDoIHave());
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            logger.Information("Starting ConfigureServices");

            services.Configure<ApplicationSettings>(Configuration);

            var keyVaultSettings = Configuration.GetSection("KeyVault").Get<KeyVaultSettings>();
            var storageSettings = Configuration.GetSection("Storage").Get<StorageSettings>();

            services.AddDataProtection()
                .SetApplicationName("nocond-identity")
                .PersistKeysToAzureBlobStorage(
                    Configuration.GetConnectionString("StorageConnectionString"),
                    storageSettings.ContainerName,
                    storageSettings.BlobName);
            
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(conf =>
                {
                    conf.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddApplicationInsightsTelemetry();

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SqlConnectionString")));

            services.AddIdentity<UserData, RoleData>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                })
                .AddRoles<RoleData>()
                .AddUserStore<IdentityUserStore>()
                .AddRoleStore<IdentityRoleStore>()
                .AddUserManager<IdentityUserManager>()
                .AddRoleManager<IdentityRoleManager>()
                .AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>()
                .AddDefaultTokenProviders()
                .AddSignInManager();

            var builder = services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/login";
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            }).AddAspNetIdentity<UserData>();

            // if (Env.IsDevelopment())
            // {
            //     builder.AddDeveloperSigningCredential();
            // }
            // else
            // {
                builder.AddKeyVaultSigningCredentials(keyVaultSettings);
            //}

            var migrationsAssembly = typeof(IdentityContext).Assembly.GetName().Name;
            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builderScope =>
                    builderScope.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString"),
                        sql =>
                        {
                            sql.MigrationsAssembly(migrationsAssembly);
                            sql.EnableRetryOnFailure(MaxSqlRetries, TimeSpan.FromSeconds(15), null);
                        });
            });
            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builderScope =>
                    builderScope.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString"),
                        sql =>
                        {
                            sql.MigrationsAssembly(migrationsAssembly);
                            sql.EnableRetryOnFailure(maxRetryCount: MaxSqlRetries, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });

                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            });

            services.AddKeyVaultTokenCreation();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSpaStaticFiles(configuration => configuration.RootPath = $"{_spaSourcePath}/build");

            services.AddApiVersioning(
                options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            logger.Information("Starting Configure");
            var settings = app.ApplicationServices.GetService<IOptions<ApplicationSettings>>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseCors();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = _spaSourcePath;

                if (env.IsDevelopment())
                    spa.UseReactDevelopmentServer("start");
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.ApplyMigrations(logger);

            logger.Information("Finished Configure");
        }
    }
}