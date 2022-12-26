using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoCond.Identity.Application.User.Data;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/password")]
    public class SetPasswordController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly SignInManager<UserData> _signInManager;

        public SetPasswordController(IdentityUserManager userManager,
            SignInManager<UserData> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(SetPasswordResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SetPasswordResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SetPasswordResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostAsync([FromBody] SetPasswordRequest request)
        {
            var result = new SetPasswordResult();

            if (!ModelState.IsValid)
            {
                result.ModelState = ModelState;

                return BadRequest(result);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);

            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                result.ModelState = ModelState;

                return BadRequest(result);
            }

            await _signInManager.RefreshSignInAsync(user);
            result.StatusMessage = "Your password has been set.";

            return Ok(result);
        }
    }
}
