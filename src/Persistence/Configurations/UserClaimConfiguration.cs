using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Persistence.Extensions;

namespace NoCond.Identity.Persistence.Configurations
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaimData>
    {
        public void Configure (EntityTypeBuilder<UserClaimData> builder)
        {
            builder
                .ToTable ("UserClaims");
        }
    }
}