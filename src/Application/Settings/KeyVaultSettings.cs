namespace NoCond.Identity.Application.Settings
{
    public class KeyVaultSettings
    {
        public string Name { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentityServerSigningKeyName { get; set; }
    }
}