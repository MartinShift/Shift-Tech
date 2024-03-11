﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Shift_Tech.Models.Account
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string Login { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
