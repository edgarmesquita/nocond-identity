using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/recovery-code/login")]
    public class LoginWithRecoveryCodeController : ControllerBase
    {
        private readonly SignInManager<UserData> _signInManager;
        private readonly ILogger<LoginWithRecoveryCodeController> _logger;

        public LoginWithRecoveryCodeController(SignInManager<UserData> signInManager,
                                               ILogger<LoginWithRecoveryCodeController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(LoginWithRecoveryCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginWithRecoveryCodeResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginWithRecoveryCodeResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostAsync([FromBody] LoginWithRecoveryCodeRequest request, [FromQuery] string returnUrl = null)
        {
            var loginWithRecoveryCodeResult = new LoginWithRecoveryCodeResult();

            if (!ModelState.IsValid)
            {
                loginWithRecoveryCodeResult.Status = SignInStatus.Invalid;
                return BadRequest(loginWithRecoveryCodeResult);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogWarning("Unable to load two-factor authentication user.");

                loginWithRecoveryCodeResult.Status = SignInStatus.Invalid;

                return StatusCode(StatusCodes.Status401Unauthorized, loginWithRecoveryCodeResult);
            }

            var recoveryCode = request.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);

                loginWithRecoveryCodeResult.Status = SignInStatus.Succeeded;
                loginWithRecoveryCodeResult.RedirectPath = returnUrl ?? Url.Content("~/");

                return Ok(loginWithRecoveryCodeResult);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);

                loginWithRecoveryCodeResult.Status = SignInStatus.IsLockedOut;
                loginWithRecoveryCodeResult.RedirectPath = "/lockout";

                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");

                loginWithRecoveryCodeResult.Status = SignInStatus.Invalid;
                loginWithRecoveryCodeResult.ModelState = ModelState;

                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
        }
    }
}
