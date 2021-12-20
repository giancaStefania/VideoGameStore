using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CG.MVC5.Stefania.Models;

namespace CG.MVC5.Stefania.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Password format is not correct")]
        public string Password { set; get; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords are different")]
        public string ConfirmPassword { set; get; }
        [Required]
        public string Token { set; get; }
    }
}