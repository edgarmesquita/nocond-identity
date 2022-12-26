using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Persistence.Extensions;

namespace NoCond.Identity.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserData>
    {
        public void Configure (EntityTypeBuilder<UserData> builder)
        {
            builder
                .ToTable ("Users");
        }
    }
}