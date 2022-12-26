using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class EnableAuthenticatorResult
    {
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        public string[] RecoveryCodes { get; set; }

        public string StatusMessage { get; set; }

        public string RedirectPath { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
