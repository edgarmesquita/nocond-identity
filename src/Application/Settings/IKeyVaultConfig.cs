namespace NoCond.Identity.Application.Settings
{
    public interface IKeyVaultConfig
    {
        string KeyVaultName { get; }

        string KeyVaultCertificateName { get; }

        int KeyVaultRolloverHours { get; }
    }
}