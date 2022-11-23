using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    public class ExamQuestion
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int Degree { get; set; }
        public virtual Question Question { get; set; }
        public virtual Exam Exam { get; set; }
    }
}