using System.Linq;
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
    [Route("internal/recovery-code/generate")]
    public class GenerateRecoveryCodesController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly ILogger<GenerateRecoveryCodesController> _logger;

        public GenerateRecoveryCodesController(IdentityUserManager userManager,
            ILogger<GenerateRecoveryCodesController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(GenerateRecoveryCodesResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenerateRecoveryCodesResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GenerateRecoveryCodesResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostAsync()
        {
            var result = new GenerateRecoveryCodesResult();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                result.StatusMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'.";

                return NotFound(result);
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!isTwoFactorEnabled)
            {
                result.StatusMessage = $"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.";

                return Unauthorized(result);
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            result.RecoveryCodes = recoveryCodes.ToArray();

            _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
            result.StatusMessage = "You have generated new recovery codes.";
            result.RedirectPath = "/show-recovery-codes";

            return Ok(result);
        }
    }
}
