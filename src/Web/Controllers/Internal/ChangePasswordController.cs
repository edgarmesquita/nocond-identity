using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/password")]
    public class ChangePasswordController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly SignInManager<UserData> _signInManager;
        private readonly ILogger<ChangePasswordController> _logger;

        public ChangePasswordController(IdentityUserManager userManager,
            SignInManager<UserData> signInManager,
            ILogger<ChangePasswordController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(ChangePasswordResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ChangePasswordResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ChangePasswordResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostAsync(ChangePasswordRequest request)
        {
            var result = new ChangePasswordResult();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                result.StatusMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'.";

                return NotFound(result);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                result.ModelState = ModelState;

                return BadRequest(result);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            result.StatusMessage = "Your password has been changed.";

            return Ok(result);
        }
    }
}
