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
    [Route("internal/personal-data/delete")]
    public class DeletePersonalDataController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly SignInManager<UserData> _signInManager;
        private readonly ILogger<DeletePersonalDataController> _logger;

        public DeletePersonalDataController(IdentityUserManager userManager,
            SignInManager<UserData> signInManager,
            ILogger<DeletePersonalDataController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(DeletePersonalDataResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DeletePersonalDataResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DeletePersonalDataResult), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostAsync([FromBody] DeletePersonalDataRequest request)
        {
            var result = new DeletePersonalDataResult();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                result.StatusMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'.";
                return NotFound(result);
            }

            var requirePassword = await _userManager.HasPasswordAsync(user);
            if (requirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");

                    result.ModelState = ModelState;
                    return Unauthorized(result);
                }
            }

            var resultDelete = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!resultDelete.Succeeded)
            {
                result.StatusMessage = $"Unexpected error occurred deleting user with ID '{userId}'.";
                return BadRequest(result);
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);
            result.RedirectPath = "~/";
            result.StatusMessage = $"User with ID '{userId}' deleted themselves.";

            return Ok(result);
        }
    }
}
