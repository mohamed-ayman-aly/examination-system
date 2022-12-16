using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    [Table("SubQuestions")]
    public class SubQuestion
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Heading { get; set; }
        public virtual List<ExamQuestion> Questions { get; set; }
        public virtual List<SubQuestion> SubQuestions { get; set; }
        public virtual List<GroupQuestion> GroupQuestions { get; set; }
        public List<Question> Exambody() {
            List<Question> all = new List<Question>();
            foreach (var q in Questions)
                all.Add(q.Question);
            foreach (var GroupQuestion in GroupQuestions)
                foreach (var q in GroupQuestion.Questions)
                    all.Add(q.Question);
            if (SubQuestions == null || SubQuestions.Count == 0)
                return all;
            else {
                foreach (var Question in SubQuestions)
                {
                    all.AddRange(Question.Exambody());
                }
                return all;
            }
        }
        public int Degree() {
            int count = 0;
            foreach (var q in Questions) {
                count+=q.Degree;
            }
            foreach (var gq in GroupQuestions)
            {
                count += gq.Degree;
            }
            foreach (var sq in SubQuestions)
            {
                count += sq.Degree();
            }
            return count;
        }
    }
}