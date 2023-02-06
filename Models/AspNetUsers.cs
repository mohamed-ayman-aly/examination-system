﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    public class AspNetUsers:IdentityUser
    {
        [Required]
        public UserType UserType { get; set; }
        public string ImgFileName { get; set; }
        [Required(ErrorMessage = "ConfirmPassword is Required"),Compare("PasswordHash",ErrorMessage = "Passwords are not match")]
        public string ConfirmPassword { get; set; }
        public virtual List<Class> Classes { get; set; }
        [InverseProperty("Professor")]
        public virtual List<Exam> MyExams { get; set; }
        [InverseProperty("Student")]
        public virtual List<ExamStudent> Exams { get; set; }
    }
}