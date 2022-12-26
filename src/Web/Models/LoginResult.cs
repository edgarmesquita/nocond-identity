using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public enum SignInStatus
    {
        Succeeded,
        RequiresTwoFactor,
        IsLockedOut,
        Invalid
    }
    public class LoginResult
    {
        public SignInStatus Status { get; set; }
        public string RedirectPath { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}