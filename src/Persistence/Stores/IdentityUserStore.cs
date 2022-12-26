using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoCond.Identity.Application.User.Data;

namespace NoCond.Identity.Persistence.Stores
{
    public class IdentityUserStore : UserStore<UserData, RoleData, IdentityContext, int, UserClaimData, UserRoleData, UserLoginData, UserTokenData, RoleClaimData>
    {
        public IdentityUserStore(IdentityContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }
    }
}