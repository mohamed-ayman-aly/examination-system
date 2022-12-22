using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    [Table("Studentanswer")]
    public class Studentanswer
    {
        [Key]
        public Guid Id { get; set; }
        public virtual Answer Answer { get; set; }
        public virtual ExamStudent ExamStudent { get; set; }
        public virtual ExamQuestion ExamQuestion { get; set; }
        public virtual GroupQuestion GroupQuestion { get; set; }


    }
}