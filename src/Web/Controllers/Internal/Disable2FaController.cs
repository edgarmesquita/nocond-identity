using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/2fa/disable")]
    public class Disable2FaController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly ILogger<Disable2FaController> _logger;

        public Disable2FaController(IdentityUserManager userManager,
                                    ILogger<Disable2FaController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(Disable2FaResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Disable2FaResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Disable2FaResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostAsync()
        {
            var result = new Disable2FaResult();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                result.StatusMessage = $"Unexpected error occurred disabling 2FA for user with ID '{_userManager.GetUserId(User)}'.";
                return BadRequest(result);
            }

            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", _userManager.GetUserId(User));

            result.StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
            result.RedirectPath = "/twofactor-authentication";

            return Ok(result);
        }
    }
}
