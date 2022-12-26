using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Persistence.Extensions;

namespace NoCond.Identity.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<RoleData>
    {
        public void Configure (EntityTypeBuilder<RoleData> builder)
        {
            builder
                .ToTable ("Roles");
        }
    }
}