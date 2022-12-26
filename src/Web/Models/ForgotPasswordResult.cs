using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class ForgotPasswordResult
    {
        public string RedirectPath { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
