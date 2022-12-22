﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    [Table("ExamStudent")]
    public class ExamStudent
    {
        [Key]
        public Guid Id { get; set; }
        [InverseProperty("ExamStudent")]
        public virtual Exam Exam { get; set; }
        public bool Submit { get; set; }
        public int Degree { get; set; }
        public virtual AspNetUsers Student { get; set; }
        public virtual List<Studentanswer> Answers { get; set; }
        public virtual List<Question> Questionsingrops { get; set; }
    }
}