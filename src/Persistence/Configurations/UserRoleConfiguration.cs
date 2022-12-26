using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoCond.Identity.Application.User.Data;

namespace NoCond.Identity.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleData>
    {
        public void Configure (EntityTypeBuilder<UserRoleData> builder)
        {
            builder
                .ToTable ("UserRoles");

        }
    }
}