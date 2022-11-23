using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Models
{
    [Table("Questions")]
    public class Question
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Question Body is Required"),AllowHtml]
        public string QuestionBody { get; set; }
        public virtual List<Answer> Answers { get; set; }
        public virtual List<ExamQuestion> Exams { get; set; }
        public virtual Answer CorrectAnswer { get; set; }
        public virtual Class Class { get; set; }


    }
}