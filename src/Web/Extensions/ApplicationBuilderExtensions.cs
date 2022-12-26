using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NoCond.Identity.Application.Settings;
using NoCond.Identity.Persistence;
using Serilog;

namespace NoCond.Identity.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app, ILogger logger)
        {
            var settings = app.ApplicationServices.GetService<IOptions<ApplicationSettings>>();
            
            if (settings.Value.Database.MigrateOnStartup)
            {
                logger.Information("Applying migrations");

                using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var dbContext = scope.ServiceProvider.GetService<IdentityContext>();
                dbContext.Database.Migrate();

                var configurationDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                configurationDbContext.Database.Migrate();

                var apiScopes = settings.Value.IdentityServer.GetApiScopes();
                foreach (var apiScope in apiScopes)
                {
                    var original = configurationDbContext.ApiScopes
                        .FirstOrDefault(c => c.Name == apiScope.Name);
                    if (original == null)
                    {
                        configurationDbContext.ApiScopes.Add(apiScope.ToEntity());
                    }
                }
                configurationDbContext.SaveChanges();

                var clients = settings.Value.IdentityServer.GetClients();
                foreach (var client in clients)
                {
                    var original = configurationDbContext.Clients
                        .Include(o => o.AllowedScopes)
                        .Include(o => o.RedirectUris)
                        .Include(o => o.PostLogoutRedirectUris)
                        .Include(o => o.AllowedCorsOrigins)
                        .FirstOrDefault(c => c.ClientId == client.ClientId);

                    var clientEntity = client.ToEntity();
                    if (original == null)
                    {
                        configurationDbContext.Clients.Add(client.ToEntity());
                    }
                    else
                    {
                        original.ClientUri = clientEntity.ClientUri;

                        if (original.RedirectUris.FirstOrDefault()?.RedirectUri !=
                            clientEntity.RedirectUris.FirstOrDefault()?.RedirectUri)
                        {
                            original.RedirectUris.Clear();
                            original.RedirectUris = clientEntity.RedirectUris;
                        }

                        if (original.PostLogoutRedirectUris.FirstOrDefault()?.PostLogoutRedirectUri !=
                            clientEntity.PostLogoutRedirectUris.FirstOrDefault()?.PostLogoutRedirectUri)
                        {
                            original.PostLogoutRedirectUris.Clear();
                            original.PostLogoutRedirectUris = clientEntity.PostLogoutRedirectUris;
                        }

                        if (original.AllowedCorsOrigins.FirstOrDefault()?.Origin !=
                            clientEntity.AllowedCorsOrigins.FirstOrDefault()?.Origin)
                        {
                            original.AllowedCorsOrigins.Clear();
                            original.AllowedCorsOrigins = clientEntity.AllowedCorsOrigins;
                        }

                        foreach (var allowedScope in client.AllowedScopes)
                        {
                            if (original.AllowedScopes.All(s => s.Scope != allowedScope))
                            {
                                original.AllowedScopes.Add(new ClientScope
                                {
                                    Scope = allowedScope,
                                        ClientId = original.Id
                                });
                            }
                        }

                        configurationDbContext.Clients.Update(original);
                    }
                }
                configurationDbContext.SaveChanges();

                var identityResources = settings.Value.IdentityServer.GetIdentityResources();
                foreach (var identityResource in identityResources)
                {
                    var original = configurationDbContext.IdentityResources
                        .FirstOrDefault(c => c.Name == identityResource.Name);
                    if (original == null)
                    {
                        configurationDbContext.IdentityResources.Add(identityResource.ToEntity());
                    }
                }
                configurationDbContext.SaveChanges();

                var apiResources = settings.Value.IdentityServer.GetApiResources();
                foreach (var apiResource in apiResources)
                {
                    var original = configurationDbContext.ApiResources
                        .Include(o => o.Scopes)
                        .FirstOrDefault(c => c.Name == apiResource.Name);

                    if (original == null)
                    {
                        configurationDbContext.ApiResources.Add(apiResource.ToEntity());
                    }
                    else
                    {
                        if (!original.Scopes.Any())
                        {
                            foreach (var apiResourceScope in apiResource.Scopes)
                            {
                                original.Scopes.Add(new ApiResourceScope
                                {
                                    Scope = apiResourceScope,
                                        ApiResourceId = original.Id
                                });
                            }
                        }

                        configurationDbContext.Update(original);
                    }
                }
                configurationDbContext.SaveChanges();

                var persistedGrantDbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();
                persistedGrantDbContext.Database.Migrate();
            }
        }
    }
}