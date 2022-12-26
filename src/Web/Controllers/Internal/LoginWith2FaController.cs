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
    [Route("internal/2fa")]
    public class LoginWith2FaController : ControllerBase
    {
        private readonly SignInManager<UserData> _signInManager;
        private readonly ILogger<LoginWith2FaController> _logger;

        public LoginWith2FaController(SignInManager<UserData> signInManager,
                                      ILogger<LoginWith2FaController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(LoginWith2FaResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginWith2FaResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginWith2FaResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostAsync([FromBody] LoginWith2FaRequest request, [FromQuery] string returnUrl = null)
        {
            var loginWith2FaResult = new LoginWith2FaResult();

            if (!ModelState.IsValid)
            {
                loginWith2FaResult.Status = SignInStatus.Invalid;
                loginWith2FaResult.ModelState = ModelState;

                return BadRequest(loginWith2FaResult);
            }

            loginWith2FaResult.RedirectPath = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                _logger.LogWarning("Unable to load two-factor authentication user.");
                loginWith2FaResult.Status = SignInStatus.Invalid;

                return StatusCode(StatusCodes.Status401Unauthorized, loginWith2FaResult);
            }

            var authenticatorCode = request.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, request.RememberMe, request.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);

                loginWith2FaResult.Status = SignInStatus.Succeeded;
                loginWith2FaResult.RedirectPath = returnUrl;

                return Ok(loginWith2FaResult);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);

                loginWith2FaResult.Status = SignInStatus.IsLockedOut;
                loginWith2FaResult.RedirectPath = "/lockout";

                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");

                loginWith2FaResult.Status = SignInStatus.Invalid;
                loginWith2FaResult.ModelState = ModelState;

                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
        }
    }
}
