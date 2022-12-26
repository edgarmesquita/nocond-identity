using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NoCond.Identity.Web.Models
{
    public class LoginWithRecoveryCodeRequest
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }

        public ModelStateDictionary ModelState { get; set; }

        public string RedirectPath { get; set; }

        public SignInStatus Status { get; set; }
    }
}
