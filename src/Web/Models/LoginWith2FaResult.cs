using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class LoginWith2FaResult
    {
        public SignInStatus Status { get; set; }

        public string RedirectPath { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
