using System;
using System.Security.Cryptography;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Lamar;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NoCond.Identity.Application.Settings;
using NoCond.Identity.Infrastructure.Registry;
using NoCond.Identity.Infrastructure.SigningCredentials;
using NoCond.Identity.Infrastructure.Token;

namespace NoCond.Identity.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Includes the infrastructure service registry.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static ServiceRegistry IncludeInfrastructureServiceRegistry (this ServiceRegistry services)
        {
            services.IncludeRegistry<InfrastructureServiceRegistry> ();
            return services;
        }
        
        public static IServiceCollection AddKeyVaultSigningCredentials(this IServiceCollection @this)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var authenticationCallback = new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
            var keyVaultClient = new KeyVaultClient(authenticationCallback);
            @this.AddMemoryCache();
            @this.AddSingleton(keyVaultClient);
            @this.AddSingleton<AzureKeyVaultSigningCredentialsStore>();
            @this.AddSingleton<ISigningCredentialStore>(services => services.GetRequiredService<AzureKeyVaultSigningCredentialsStore>());
            @this.AddSingleton<IValidationKeysStore>(services => services.GetRequiredService<AzureKeyVaultSigningCredentialsStore>());
            return @this;
        }

        public static IIdentityServerBuilder AddKeyVaultSigningCredentials(this IIdentityServerBuilder identityServerBuilder, KeyVaultSettings keyVaultSettings)
        {
            var keyClient = new KeyClient(
                new Uri($"https://{keyVaultSettings.Name}.vault.azure.net/"),
                new ClientSecretCredential(
                    tenantId: keyVaultSettings.TenantId,
                    clientId: keyVaultSettings.ClientId,
                    clientSecret: keyVaultSettings.ClientSecret));

            Response<KeyVaultKey> response = keyClient.GetKey(keyVaultSettings.IdentityServerSigningKeyName);

            AsymmetricSecurityKey key;
            string algorithm;

            if (response.Value.KeyType == KeyType.Ec)
            {
                ECDsa ecDsa = response.Value.Key.ToECDsa();
                key = new ECDsaSecurityKey(ecDsa) { KeyId = response.Value.Properties.Version };

                // parse from curve
                if (response.Value.Key.CurveName == KeyCurveName.P256) algorithm = "ES256";
                else if (response.Value.Key.CurveName == KeyCurveName.P384) algorithm = "ES384";
                else if (response.Value.Key.CurveName == KeyCurveName.P521) algorithm = "ES521";
                else throw new NotSupportedException();
            }
            else if (response.Value.KeyType == KeyType.Rsa)
            {
                RSA rsa = response.Value.Key.ToRSA();
                key = new RsaSecurityKey(rsa) { KeyId = response.Value.Properties.Version };

                // you define
                algorithm = "PS256";
            }
            else
            {
                throw new NotSupportedException();
            }
            return identityServerBuilder.AddSigningCredential(key, algorithm);
        }
        public static IServiceCollection AddKeyVaultTokenCreation(this IServiceCollection services)
        {
            services.AddTransient<ITokenCreationService, KeyVaultTokenCreationService>();
            return services;
        }
    }
}