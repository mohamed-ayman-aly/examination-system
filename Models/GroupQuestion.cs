using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    [Table("GroupQuestions")]
    public class GroupQuestion
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int Degree { get; set; }
        public virtual List<Question> Questions { get; set; }
    }
}