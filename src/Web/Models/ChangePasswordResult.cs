using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class ChangePasswordResult
    {
        public string StatusMessage { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
