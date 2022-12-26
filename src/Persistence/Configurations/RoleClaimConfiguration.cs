using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Persistence.Extensions;

namespace NoCond.Identity.Persistence.Configurations
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaimData>
    {
        public void Configure (EntityTypeBuilder<RoleClaimData> builder)
        {
            builder
                .ToTable ("RoleClaims");
        }
    }
}