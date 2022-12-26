using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys.Cryptography;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoCond.Identity.Application.Settings;

namespace NoCond.Identity.Infrastructure.Token
{
    public class KeyVaultTokenCreationService : DefaultTokenCreationService
    {
        private readonly IOptions<ApplicationSettings> _applicationSettings;

        public KeyVaultTokenCreationService(
            IOptions<ApplicationSettings> applicationSettings,
            ISystemClock clock, IKeyMaterialService keys, 
            IdentityServerOptions options, ILogger<DefaultTokenCreationService> logger) : base(clock, keys, options, logger)
        {
            _applicationSettings = applicationSettings;
        }

        protected override async Task<string> CreateJwtAsync(JwtSecurityToken jwt)
        {
            var plaintext = $"{jwt.EncodedHeader}.{jwt.EncodedPayload}";

            byte[] hash;
            using(var hasher = CryptoHelper.GetHashAlgorithmForSigningAlgorithm(jwt.SignatureAlgorithm))
            {
                hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
            }

            var keyVaultSettings = _applicationSettings.Value.KeyVault;
            var cryptoClient = new CryptographyClient(
                new Uri($"https://{keyVaultSettings.Name}.vault.azure.net/keys/{keyVaultSettings.IdentityServerSigningKeyName}"),
                new ClientSecretCredential(
                    tenantId: keyVaultSettings.TenantId,
                    clientId: keyVaultSettings.ClientId,
                    clientSecret: keyVaultSettings.ClientSecret));

            var signResult = await cryptoClient.SignAsync(new SignatureAlgorithm(jwt.SignatureAlgorithm), hash);

            return $"{plaintext}.{Base64UrlTextEncoder.Encode(signResult.Signature)}";
        }
    }
}