﻿using System.ComponentModel.DataAnnotations;

namespace NoCond.Identity.Web.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
