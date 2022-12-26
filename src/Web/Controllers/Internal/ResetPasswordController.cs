using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/password/reset")]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;

        public ResetPasswordController(IdentityUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(ResetPasswordResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResetPasswordResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResetPasswordResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostAsync([FromBody] ResetPasswordRequest request)
        {
            var resetPasswordResult = new ResetPasswordResult();

            if (!ModelState.IsValid)
            {
                return BadRequest(resetPasswordResult);
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                resetPasswordResult.RedirectPath = "/reset-password-confirmation";

                return NotFound(resetPasswordResult);
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
            if (result.Succeeded)
            {
                resetPasswordResult.RedirectPath = "/reset-password-confirmation";

                return Ok(resetPasswordResult);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            resetPasswordResult.ModelState = ModelState;

            return BadRequest(resetPasswordResult);
        }
    }
}
