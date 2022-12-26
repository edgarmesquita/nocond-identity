using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class DeletePersonalDataResult
    {
        public string StatusMessage { get; set; }

        public string RedirectPath { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
