using examination_system.Models.View_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Models
{
    [Table("Answers")]
    public class Answer
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Answer Body is Required"), AllowHtml]
        public string AnswerBody { get; set; } 

    }
}