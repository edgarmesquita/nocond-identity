using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public enum RegisterStatus
    {
        Succeeded,
        SucceededWithConfirmEmail,
        Invalid
    }

    public class RegisterResult
    {
        public RegisterStatus Status { get; set; }

        public string RedirectPath { get; set; }

        public string ConfirmEmailPath { get; set; }

        public string Email { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
