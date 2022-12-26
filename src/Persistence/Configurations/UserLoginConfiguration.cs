using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoCond.Identity.Application.User.Data;

namespace NoCond.Identity.Persistence.Configurations
{
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLoginData>
    {
        public void Configure (EntityTypeBuilder<UserLoginData> builder)
        {
            builder
                .ToTable ("UserLogins");
        }
    }
}