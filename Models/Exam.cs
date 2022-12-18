using examination_system.Models.View_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    [Table("Exams")]
    public class Exam
    {
        [Key]
        public Guid Id { get; set; }
        [Unique(ErrorMessage = "this Exam name is already exist"),Required(ErrorMessage = "Exam name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Exam date is Required"),Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Exam date is Required")]
        public int Duration { get; set; }
        public bool Submit { get; set; }
        public virtual AspNetUsers Professor { get; set; }
        public virtual Class Class { get; set; }
        public virtual List<ExamQuestion> Questions { get; set; }
        public virtual List<SubQuestion> SubQuestions { get; set; }
        public virtual List<GroupQuestion> GroupQuestions { get; set; }
        public List<Question> Exambody()
        {
            List<Question> all = new List<Question>();
            foreach(var q in Questions)
                all.Add(q.Question);
            foreach (var GroupQuestion in GroupQuestions)
                foreach (var q in GroupQuestion.Questions)
                    all.Add(q.Question);
            if (SubQuestions == null || SubQuestions.Count == 0)
                return all;
            else
            {
                foreach (var Question in SubQuestions)
                {
                    all.AddRange(Question.Exambody());
                }
                return all;
            }
        }
    }
}