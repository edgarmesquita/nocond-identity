using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/authenticator/enable")]
    public class EnableAuthenticatorController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly ILogger<EnableAuthenticatorController> _logger;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public EnableAuthenticatorController(IdentityUserManager userManager,
            ILogger<EnableAuthenticatorController> logger,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(EnableAuthenticatorResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EnableAuthenticatorResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EnableAuthenticatorResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostAsync(EnableAuthenticatorRequest request)
        {
            var result = new EnableAuthenticatorResult();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                result.StatusMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'.";
                return NotFound(result);
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, result);

                return BadRequest(result);
            }

            // Strip spaces and hypens
            var verificationCode = request.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid.");
                result.ModelState = ModelState;

                await LoadSharedKeyAndQrCodeUriAsync(user, result);

                return BadRequest(result);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var userId = await _userManager.GetUserIdAsync(user);
            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

            result.StatusMessage = "Your authenticator app has been verified.";

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                result.RecoveryCodes = recoveryCodes.ToArray();
                result.RedirectPath = "/show-recovery-codes";

                return Ok(result);
            }
            else
            {
                result.RedirectPath = "/two-factor-authentication";
                return Ok(result);
            }
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(UserData user, EnableAuthenticatorResult result)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            result.SharedKey = FormatKey(unformattedKey);

            var email = await _userManager.GetEmailAsync(user);
            result.AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("NoCond.Identity"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
