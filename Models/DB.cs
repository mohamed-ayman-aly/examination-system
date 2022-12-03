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
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamStudent> ExamStudent { get; set; }
        public DbSet<Studentanswer> Studentsanswers { get; set; }
        public DbSet<SubQuestion> SubQuestions { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<GroupQuestion> GroupQuestions { get; set; }
    }
}