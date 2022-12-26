namespace NoCond.Identity.Application.Settings
{
    public class DatabaseSettings
    {
        public bool MigrateOnStartup { get; set; }

        public bool UseInternalServiceProvider { get; set; }

        public bool UseTransactions { get; set; }
    }
}