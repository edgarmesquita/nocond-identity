using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/register")]
    public class RegisterController : ControllerBase
    {
        private readonly SignInManager<UserData> _signInManager;
        private readonly IdentityUserManager _userManager;
        private readonly ILogger<RegisterController> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterController(SignInManager<UserData> signInManager,
            IdentityUserManager userManager,
            ILogger<RegisterController> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;

        }

        [HttpPost("")]
        [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromForm] RegisterRequest request, [FromQuery] string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var result = new RegisterResult();

            if (ModelState.IsValid)
            {
                var user = new UserData { UserName = request.Email, Email = request.Email };
                var identityResult = await _userManager.CreateAsync(user, request.Password);
                if (identityResult.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        _logger.LogInformation("SignIn requires email confirmation.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = $"/register/confirm-email?userId={user.Id}&code={code}&returnUrl={returnUrl}";

                        result.Status = RegisterStatus.SucceededWithConfirmEmail;
                        result.Email = request.Email;
                        result.RedirectPath = returnUrl;
                        result.ConfirmEmailPath = callbackUrl;

                        return Ok(result);
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        _logger.LogInformation("User logged in.");

                        result.Status = RegisterStatus.Succeeded;
                        result.RedirectPath = returnUrl;

                        return Ok(result);
                    }
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            result.Status = RegisterStatus.Invalid;
            result.ModelState = ModelState;
            return BadRequest(result);
        }
    }
}
