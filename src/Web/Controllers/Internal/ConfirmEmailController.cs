using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NoCond.Identity.Application.User.Managers;
using NoCond.Identity.Web.Models;

namespace NoCond.Identity.Web.Controllers.Internal
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("internal/email/confirm")]
    public class ConfirmEmailController : ControllerBase
    {
        private readonly IdentityUserManager _userManager;
        private readonly ILogger<RegisterController> _logger;

        public ConfirmEmailController(IdentityUserManager userManager, ILogger<RegisterController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync(string userId, string code)
        {
            var confirmResult = new ConfirmEmailResult();

            if (userId == null || code == null)
                confirmResult.RedirectPath = Url.Content("~/");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
                return Ok();

            return BadRequest(confirmResult);
        }
    }
}
