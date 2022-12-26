using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoCond.Identity.Application.User.Data;

namespace NoCond.Identity.Persistence.Configurations
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserTokenData>
    {
        public void Configure (EntityTypeBuilder<UserTokenData> builder)
        {
            builder
                .ToTable ("UserTokens");
        }
    }
}