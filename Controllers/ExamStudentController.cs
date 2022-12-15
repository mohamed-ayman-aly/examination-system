using examination_system.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Controllers
{
    public class ExamStudentController : Controller
    {
        DB DB = new DB();
        [HttpPost,ValidateAntiForgeryToken,Authorize(Roles = "superadmin,student")]
        public ActionResult Index(string name)
        {
            DB = new DB();
            var userid = User.Identity.GetUserId();
            var exam = DB.Exams.FirstOrDefault(e => e.Name == name);
            var examstudent = DB.ExamStudent.FirstOrDefault(es => es.Exam.Id == exam.Id && es.Student.Id == userid);
            if (examstudent == null)
            {
                Guid id = Guid.NewGuid();
                DB.ExamStudent.Add(new ExamStudent
                {
                    Id = id,
                    Exam = exam,
                    Student = DB.Users.FirstOrDefault(u => u.Id == userid)
                });
                DB.SaveChanges();
                ViewBag.ExamStudentId = id;
            }
            else {
                ViewBag.ExamStudentId = examstudent.Id;
                ViewBag.ansers = examstudent.Answers.ToList();
            }
            if (DateTime.Now > exam.Date && DateTime.Now < exam.Date.AddMinutes(exam.Duration))
                return View(exam);
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "superadmin,student")]
        public bool AddAnswer(string e,string q,string ans)
        {
            DB = new DB();
            var myExamStudent= DB.ExamStudent.FirstOrDefault(es => es.Id.ToString() == e);
            DateTime examend = myExamStudent.Exam.Date.AddMinutes(myExamStudent.Exam.Duration);
            if (examend > DateTime.Now)
            {
                var Answers = myExamStudent.Answers;
                foreach (var Studentanswer in Answers.ToList())
                {
                    if (Studentanswer.Answer.Question.Id.ToString() == q)
                    {
                        Studentanswer.Answer = DB.Answers.FirstOrDefault(a => a.Id.ToString() == ans);
                        DB.SaveChanges();
                        return true;
                    }
                }
                Answers.Add(new Studentanswer {
                    Answer=DB.Answers.FirstOrDefault(a => a.Id.ToString() == ans),
                    ExamStudent=myExamStudent,
                    Id=Guid.NewGuid()
                });
                DB.SaveChanges();
                return true;
            }
            return false;
        }
    }
}