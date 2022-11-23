using System;
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
        public virtual Exam Exam { get; set; }
        public virtual AspNetUsers Student { get; set; }
        public virtual List<Studentanswer> Answers { get; set; }
    }
}