using System.ComponentModel.DataAnnotations;

namespace NoCond.Identity.Web.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }
}
