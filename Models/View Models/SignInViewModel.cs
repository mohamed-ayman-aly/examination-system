using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace examination_system.Models.View_Models
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Username is Required"),Unique(ErrorMessage = "This username is already exist")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is Required"),DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "ConfirmPassword is Required"), DataType(DataType.Password),Compare("Password",ErrorMessage = "Passwords are not match")]
        public string ConfirmPassword { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "PhoneNumber is Required"), DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        public HttpPostedFileBase ImgFile { get; set; }
    }
}