using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NoCond.Identity.Application.Settings;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Persistence.Configurations;

namespace NoCond.Identity.Persistence
{
    public class IdentityContext : IdentityDbContext<UserData, RoleData, int, UserClaimData, UserRoleData, UserLoginData, RoleClaimData, UserTokenData>
    {
        private readonly IOptions<ApplicationSettings> applicationSettings;

        public IdentityContext (DbContextOptions<IdentityContext> options, IOptions<ApplicationSettings> applicationSettings) : base (options)
        {
            this.applicationSettings = applicationSettings;
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring (optionsBuilder);
            if (applicationSettings.Value.Database.UseInternalServiceProvider) { }
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating (modelBuilder);

            modelBuilder.ApplyConfiguration (new UserConfiguration ());
            modelBuilder.ApplyConfiguration (new RoleConfiguration ());
            modelBuilder.ApplyConfiguration (new RoleClaimConfiguration ());
            modelBuilder.ApplyConfiguration (new UserClaimConfiguration ());
            modelBuilder.ApplyConfiguration (new UserLoginConfiguration ());
            modelBuilder.ApplyConfiguration (new UserRoleConfiguration ());
            modelBuilder.ApplyConfiguration (new UserTokenConfiguration ());
        }
    }
}