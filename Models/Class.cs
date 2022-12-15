using examination_system.Models.View_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    [Table("Classes")]
    public class Class
    {
        [Required(ErrorMessage = "Class Name is Required")]
        [Unique(ErrorMessage = "This name is already exists")]
        public string Name { get; set; }
        [Key]
        public Guid Id { get; set; }
        
        public virtual List<Question> Questions { get; set; }
        public virtual List<AspNetUsers> Students { get; set; }
        [InverseProperty("Class")]
        public virtual List<Exam> Exams { get; set; }
    }
}