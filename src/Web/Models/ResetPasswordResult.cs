using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class ResetPasswordResult
    {
        public string RedirectPath { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
