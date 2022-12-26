using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;

namespace NoCond.Identity.Application.Settings
{
    public class ApiResourceSettings
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<string> Scopes { get; set; } = new List<string>();
    }
    public class ClientSettings
    {
        public bool Enabled { get; set; } = true;
        public string ClientId { get; set; }
        public string ProtocolType { get; set; } = IdentityServerConstants.ProtocolTypes.OpenIdConnect;
        public bool RequireClientSecret { get; set; } = true;
        public string ClientName { get; set; }
        public List<string> ClientSecrets { get; set; } = new List<string>();
        public string Description { get; set; }
        public string ClientUri { get; set; }
        public string LogoUri { get; set; }
        public bool RequireConsent { get; set; } = false;
        public bool AllowRememberConsent { get; set; } = true;
        public List<string> AllowedGrantTypes { get; set; } = new List<string>();
        public bool RequirePkce { get; set; } = true;
        public bool AllowPlainTextPkce { get; set; } = false;
        public bool RequireRequestObject { get; set; } = false;
        public bool AllowAccessTokensViaBrowser { get; set; } = false;
        public List<string> RedirectUris { get; set; } = new List<string>();
        public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();
        public string FrontChannelLogoutUri { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public string BackChannelLogoutUri { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; } = true;
        public bool AllowOfflineAccess { get; set; } = false;
        public List<string> AllowedScopes { get; set; } = new List<string>();
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; } = false;
        public int IdentityTokenLifetime { get; set; } = 300;
        public List<string> AllowedIdentityTokenSigningAlgorithms { get; set; } = new List<string>();
        public int AccessTokenLifetime { get; set; } = 3600;
        public int AuthorizationCodeLifetime { get; set; } = 300;
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public int? ConsentLifetime { get; set; } = null;
        public List<string> AllowedCorsOrigins { get; set; } = new List<string>();
    }

    public class IdentityResourceSettings
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<string> UserClaims { get; set; } = new List<string>();
    }
    public class IdentityServerSettings
    {
        public Dictionary<string, ApiResourceSettings> ApiResources { get; set; } = new Dictionary<string, ApiResourceSettings>();
        public Dictionary<string, ClientSettings> Clients { get; set; } = new Dictionary<string, ClientSettings>();
        public Dictionary<string, IdentityResourceSettings> IdentityResources { get; set; } = new Dictionary<string, IdentityResourceSettings>();
        public List<Client> GetClients()
        {
            return Clients.Values.Select(c => new Client
            {
                ClientId = c.ClientId,
                ClientName = c.ClientName,
                ClientSecrets = c.ClientSecrets.Any() ? c.ClientSecrets.Select(s => new Secret(s.Sha256())).ToList() : new List<Secret>(),
                ClientUri = c.ClientUri,
                AllowedGrantTypes = c.AllowedGrantTypes,
                RequireClientSecret = c.RequireClientSecret,
                RedirectUris = c.RedirectUris,
                PostLogoutRedirectUris = c.PostLogoutRedirectUris,
                AllowedCorsOrigins = c.AllowedCorsOrigins,
                AllowedScopes = c.AllowedScopes,
                AllowAccessTokensViaBrowser = c.AllowAccessTokensViaBrowser
            }).ToList();
        }

        public List<IdentityResource> GetIdentityResources()
        {
            var result = new List<IdentityResource>();
            foreach (var identityResource in IdentityResources)
            {
                var resource = identityResource.Value.Name switch
                {
                    IdentityServerConstants.StandardScopes.OpenId => new IdentityResources.OpenId(),
                    IdentityServerConstants.StandardScopes.Profile => new IdentityResources.Profile(),
                    _ => new IdentityResource(identityResource.Value.Name, identityResource.Value.DisplayName,
                        identityResource.Value.UserClaims)
                };
                foreach (var claim in identityResource.Value.UserClaims)
                {
                    if (!resource.UserClaims.Contains(claim))
                    {
                        resource.UserClaims.Add(claim);
                    }
                }
                result.Add( resource);
            }

            return result;
        }
        public List<ApiResource> GetApiResources()
        {
            return ApiResources.Select(o =>
            {
                var apiResource = new ApiResource(o.Value.Name, o.Value.DisplayName);
                foreach (var scope in o.Value.Scopes)
                {
                    apiResource.Scopes.Add(scope);
                }

                return apiResource;
            }).ToList();
        }

        public List<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "read",   displayName: "Read your data."),
                new ApiScope(name: "write",  displayName: "Write your data."),
                new ApiScope(name: "delete", displayName: "Delete your data."),
                new ApiScope(name: "manage", displayName: "Provides administrative access to all data.")
            };
        }
    }
}