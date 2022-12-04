using examination_system.Models.View_Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    public enum UserType
    {
        student,professor,admin
    }
    public class DB:IdentityDbContext<AspNetUsers>
    {
        public DB():base("Examination System") { }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<ExamStudent> ExamStudent { get; set; }
        public virtual DbSet<Studentanswer> Studentsanswers { get; set; }
        public virtual DbSet<SubQuestion> SubQuestions { get; set; }
        public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }
        public virtual DbSet<GroupQuestion> GroupQuestions { get; set; }
    }
}