using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoCond.Identity.Application.User.Data;

namespace NoCond.Identity.Application.User.Managers
{
    public class IdentityUserManager : UserManager<UserData>
    {
        public IdentityUserManager (IUserStore<UserData> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<UserData> passwordHasher, 
            IEnumerable<IUserValidator<UserData>> userValidators, 
            IEnumerable<IPasswordValidator<UserData>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, IServiceProvider services, 
            ILogger<IdentityUserManager> logger) 
            : base (store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) { }
    }
}