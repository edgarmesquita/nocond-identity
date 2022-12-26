using System.ComponentModel.DataAnnotations;

namespace NoCond.Identity.Web.Models
{
    public class DeletePersonalDataRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
