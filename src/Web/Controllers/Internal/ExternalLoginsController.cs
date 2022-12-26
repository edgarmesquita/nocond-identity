using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/login/external")]
    public class ExternalLoginsController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly SignInManager<UserData> _signInManager;

        public ExternalLoginsController(IdentityUserManager userManager,
            SignInManager<UserData> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync()
        {
            var result = new ExternalLoginsResult();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID 'user.Id'.");
            }

            result.CurrentLogins = await _userManager.GetLoginsAsync(user);
            result.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => result.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            result.ShowRemoveButton = user.PasswordHash != null || result.CurrentLogins.Count > 1;

            return Ok(result);
        }

        [HttpPost("remove-login")]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var resultExternalLogins = new ExternalLoginsResult();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                resultExternalLogins.StatusMessage = $"Unable to load user with ID 'user.Id'.";
                return NotFound(resultExternalLogins);
            }

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                resultExternalLogins.StatusMessage = "The external login was not removed.";
                return BadRequest(resultExternalLogins);
            }

            await _signInManager.RefreshSignInAsync(user);
            resultExternalLogins.StatusMessage = "The external login was removed.";
            return Ok(resultExternalLogins);
        }

        [HttpPost("link-login")]
        public async Task<IActionResult> PostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await _signInManager.SignOutAsync();

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = "/external-logins/link-login-callback";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet("link-login-callback")]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExternalLoginsResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLinkLoginCallbackAsync()
        {
            var resultExternalLogins = new ExternalLoginsResult();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                resultExternalLogins.StatusMessage = $"Unable to load user with ID 'user.Id'.";
                return NotFound(resultExternalLogins);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id.ToString());
            if (info == null)
            {
                resultExternalLogins.StatusMessage = $"Unexpected error occurred loading external login info for user with ID '{user.Id}'.";
                return Unauthorized(resultExternalLogins);
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                resultExternalLogins.StatusMessage = "The external login was not added. External logins can only be associated with one account.";
                return BadRequest(resultExternalLogins);
            }

            // Clear the existing external cookie to ensure a clean login process
            await _signInManager.SignOutAsync();

            resultExternalLogins.StatusMessage = "The external login was added.";
            return Ok(resultExternalLogins);
        }
    }
}
