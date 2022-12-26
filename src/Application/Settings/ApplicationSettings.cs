namespace NoCond.Identity.Application.Settings
{
    public class ApplicationSettings
    {
        public DatabaseSettings Database { get; set; }
        public IdentityServerSettings IdentityServer { get; set; }
        public KeyVaultSettings KeyVault { get; set; }
        
        public StorageSettings Storage { get; set; }
    }
}