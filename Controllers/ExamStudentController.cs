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
            var examstudent = DB.ExamStudent.Where(es => es.Exam.Id == exam.Id && es.Student.Id == userid);
            if (examstudent.Count() == 0)
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
                ViewBag.ExamStudentId = examstudent.FirstOrDefault().Id;
            }
            if(DateTime.Now > exam.Date && DateTime.Now < exam.Date.AddMinutes(exam.Duration))
                return View(exam);
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "superadmin,student")]
        public bool AddAnswer() {
            DB = new DB();
            DB.ExamStudent.
            return true;
        }
    }
}