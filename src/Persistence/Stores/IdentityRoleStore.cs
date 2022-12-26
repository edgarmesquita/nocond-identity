using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoCond.Identity.Application.User.Data;

namespace NoCond.Identity.Persistence.Stores
{
    public class IdentityRoleStore : RoleStore<RoleData, IdentityContext, int>
    {
        public IdentityRoleStore (IdentityContext context, IdentityErrorDescriber describer = null) : base (context, describer)
        { }
    }
}