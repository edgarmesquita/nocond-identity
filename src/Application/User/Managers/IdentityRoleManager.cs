using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NoCond.Identity.Application.User.Data;

namespace NoCond.Identity.Application.User.Managers
{
    public class IdentityRoleManager : RoleManager<RoleData>
    {
        public IdentityRoleManager (
            IRoleStore<RoleData> store, 
            IEnumerable<IRoleValidator<RoleData>> roleValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            ILogger<IdentityRoleManager> logger) 
            : base (store, roleValidators, keyNormalizer, errors, logger) { }
    }
}