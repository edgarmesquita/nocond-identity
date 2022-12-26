using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class Disable2FaResult
    {
        public string StatusMessage { get; set; }

        public string RedirectPath { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
