using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/email")]
    public class EmailController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly SignInManager<UserData> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailController(IdentityUserManager userManager,
            SignInManager<UserData> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost("change-email")]
        [ProducesResponseType(typeof(EmailResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmailResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmailResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostChangeEmailAsync([FromBody] EmailRequest request)
        {
            var result = new EmailResult();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                result.StatusMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'.";

                return NotFound(result);
            }

            if (!ModelState.IsValid)
            {
                result.IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                result.ModelState = ModelState;

                return BadRequest(result);
            }

            var email = await _userManager.GetEmailAsync(user);
            if (request.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
                var callbackUrl = $"/confirm-email-change?userId{userId}&email={request.NewEmail}&code={code}";

                await _emailSender.SendEmailAsync(
                    request.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                result.StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return Ok(result);
            }

            result.StatusMessage = "Your email is unchanged.";
            return BadRequest(result);
        }

        [HttpPost("send-verification-email")]
        [ProducesResponseType(typeof(EmailResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EmailResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(EmailResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostSendVerificationEmailAsync()
        {
            var result = new EmailResult();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                result.StatusMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'.";
                return NotFound(result);
            }

            if (!ModelState.IsValid)
            {
                result.IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                result.ModelState = ModelState;

                return BadRequest(result);
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = $"/confirm-email?userId={userId}&code={code}";

            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            result.StatusMessage = "Verification email sent. Please check your email.";
            return Ok(result);
        }
    }
}
