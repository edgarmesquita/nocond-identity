using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;

namespace NoCond.Identity.Application
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<UserData, RoleData>
    {
        public AppClaimsPrincipalFactory(IdentityUserManager userManager, IdentityRoleManager roleManager, IOptions<IdentityOptions> options) 
            : base(userManager, roleManager, options)
        {
        }
    }
}