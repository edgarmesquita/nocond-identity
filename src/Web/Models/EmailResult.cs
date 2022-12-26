using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class EmailResult
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public string StatusMessage { get; set; }

        public ModelStateDictionary ModelState { get; set; }
    }
}
