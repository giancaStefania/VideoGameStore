using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CG.MVC5.Stefania.Models
{
    public class User
    {
        public int IdUser { set; get; }
        public string Name { set; get; }
        public string Surname { set; get; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { set; get; }
        [Required]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Password format is not correct")]
        public string Password { set; get; }
       
        [Compare("Password", ErrorMessage = "Password is not the same")]
        public string ConfirmPassword { set; get; }

    }
}