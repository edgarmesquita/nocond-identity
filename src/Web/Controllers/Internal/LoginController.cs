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
    [Route("internal/login")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<UserData> _userManager;
        private readonly SignInManager<UserData> _signInManager;
        private readonly ILogger<LoginController> _logger;

        public LoginController(SignInManager<UserData> signInManager,
            ILogger<LoginController> logger,
            UserManager<UserData> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LoginResult), StatusCodes.Status428PreconditionRequired)]
        public async Task<IActionResult> Post([FromBody] LoginRequest request, [FromQuery] string returnUrl = null)
        {
            var result = new LoginResult();
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var signInResult = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, lockoutOnFailure : false);
                if (signInResult.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    result.Status = SignInStatus.Succeeded;
                    result.RedirectPath = returnUrl ?? Url.Content("~/");
                    return Ok(result);
                }
                if (signInResult.RequiresTwoFactor)
                {
                    result.Status = SignInStatus.RequiresTwoFactor;
                    result.RedirectPath = $"/login/with2fa?ReturnUrl={returnUrl}&RememberMe={request.RememberMe}";

                    return StatusCode(StatusCodes.Status428PreconditionRequired, result);
                }
                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    result.Status = SignInStatus.IsLockedOut;
                    result.RedirectPath = "/lockout";

                    return StatusCode(StatusCodes.Status401Unauthorized, result);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    result.Status = SignInStatus.Invalid;
                    result.ModelState = ModelState;

                    return StatusCode(StatusCodes.Status401Unauthorized, result);
                }
            }
            result.Status = SignInStatus.Invalid;
            result.ModelState = ModelState;
            return BadRequest(result);
        }
    }
}