using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/authenticator/reset")]
    public class ResetAuthenticatorController : ControllerBase
    {
        IdentityUserManager _userManager;
        private readonly SignInManager<UserData> _signInManager;
        ILogger<ResetAuthenticatorController> _logger;

        public ResetAuthenticatorController(IdentityUserManager userManager,
            SignInManager<UserData> signInManager,
            ILogger<ResetAuthenticatorController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(ResetAuthenticatorResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResetAuthenticatorResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostAsync()
        {
            var result = new ResetAuthenticatorResult();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                result.StatusMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'.";

                return NotFound(result);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

            await _signInManager.RefreshSignInAsync(user);

            result.StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";
            result.RedirectPath = "/enable-authenticator";

            return Ok(result);
        }
    }
}
