using examination_system.Models;
using examination_system.Models.View_Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Controllers
{
    [Authorize(Roles = "professor,superadmin")]
    public class ExamController : Controller
    {
        static DB DB = new DB();
        static UserStore<AspNetUsers> UserStore = new UserStore<AspNetUsers>(DB);
        static UserManager<AspNetUsers> userManager = new UserManager<AspNetUsers>(UserStore);
        public ActionResult Index()
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            string s = User.Identity.GetUserId();
            return View(DB.Exams.Where(e => e.Professor.Id == s).ToList());
        }
        public string Search(string SearchName = "")
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var exams = DB.Exams.Where(e => e.Name.Contains(SearchName)).ToList();
            string ret = "";
            foreach (var e in exams)
            {
                ret += PartialView("_exam", e).RenderToString();
            }
            return ret;
        }
        public ActionResult Add()
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            ViewBag.Class = DB.Classes.ToList();
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(string Classid, Exam NewExame)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            NewExame.Id = System.Guid.NewGuid();
            NewExame.Class = DB.Classes.FirstOrDefault(c => c.Id.ToString() == Classid);
            NewExame.Professor = userManager.FindById(User.Identity.GetUserId());
            DB.Exams.Add(NewExame);
            if (!ModelState.IsValid)
            {
                ViewBag.Class = DB.Classes.ToList();
                return View(NewExame);
            }

            DB.SaveChanges();
            return RedirectToAction("Index");
        }
        public bool Delete(string Id)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var e = DB.Exams.FirstOrDefault(u => u.Id.ToString() == Id);
            DB.Exams.Remove(e);
            try
            {
                DB.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public ActionResult Edit(string id)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == id);
            if (userManager.FindById(User.Identity.GetUserId()).Equals(myexam.Professor))
            {
                ViewBag.exambody = myexam.Questions.ToList();
                ViewBag.myexamGroupQuestions = myexam.GroupQuestions.ToList();
                ViewBag.myexamSubQuestions = myexam.SubQuestions.ToList();
                var myexambody = myexam.Exambody();
                var questions = DB.Questions.Where(q => q.Class.Id == myexam.Class.Id).ToList();
                List<Question> questionbody = new List<Question>();
                foreach (var question in questions)
                {
                    if (!(myexambody.Contains(question)))
                        questionbody.Add(question);
                }
                ViewBag.questionbody = questionbody;
                return View(myexam);
            }
            return RedirectToAction("Index");
        }
        public string Guid()
        {
            return System.Guid.NewGuid().ToString();
        }
        public void AddGrop2Exam(string e, int deg)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == e);
            myexam.GroupQuestions.Add(new GroupQuestion { Id = new Guid(), Degree = deg });
        }
        public void RemoveGrop2Exam(string e, string id)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == e);
            myexam.GroupQuestions.Remove(myexam.GroupQuestions.FirstOrDefault(grop => grop.Id.ToString() == id));
        }
        public void AddQuestion2Grop(string e, string grop, string q)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == e);
            var myquestion = DB.Questions.FirstOrDefault(ex => ex.Id.ToString() == q);
            var mygrop = DB.GroupQuestions.FirstOrDefault(ex => ex.Id.ToString() == grop);
            mygrop.Questions.Add(new ExamQuestion { Id = new Guid(), Exam = myexam, Question = myquestion }); ;
            DB.SaveChanges();
        }
        public void RemoveQuestion2Grop(string grop, string q)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var mygrop = DB.GroupQuestions.FirstOrDefault(ex => ex.Id.ToString() == grop);
            mygrop.Questions.Remove(mygrop.Questions.FirstOrDefault(ex => ex.Id.ToString() == q)); ;
            DB.SaveChanges();
        }
        public void AddGrop2Sub(string sub, int deg)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var mysub = DB.SubQuestions.FirstOrDefault(ex => ex.Id.ToString() == sub);
            mysub.GroupQuestions.Add(new GroupQuestion { Id = new Guid(), Degree = deg });
            DB.SaveChanges();
        }
        public void RemoveGrop2Sub(string sub, string grop)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var mysub = DB.SubQuestions.FirstOrDefault(ex => ex.Id.ToString() == sub);
            mysub.GroupQuestions.Remove(mysub.GroupQuestions.FirstOrDefault(ex => ex.Id.ToString() == grop));
            DB.SaveChanges();
        }


        /*mohsening*/





        public void AddSub2Exam(string e, string id, string head)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == e);
            Guid Id = new Guid(id);
            var mysub = myexam.SubQuestions.FirstOrDefault(sub => sub.Id == Id);
            if (mysub == null)
            {
                mysub = new SubQuestion { Id = Id, Heading = head };
                DB.SubQuestions.Add(mysub);
                DB.SaveChanges();
                myexam.SubQuestions.Add(mysub);
                DB.SaveChanges();
            }
            else
            {
                mysub.Heading = head;
                DB.SaveChanges();
            }
        }
        public void AddSub2Sub(string sub, string id, string head)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var mysub = DB.SubQuestions.FirstOrDefault(ex => ex.Id.ToString() == sub);
            Guid Id = new Guid(id);
            var newsub = mysub.SubQuestions.FirstOrDefault(s => s.Id == Id);
            if (newsub == null)
            {
                newsub = new SubQuestion { Id = new Guid(id), Heading = head };
                DB.SubQuestions.Add(newsub);
                DB.SaveChanges();
                mysub.SubQuestions.Add(newsub);
                DB.SaveChanges();
            }
            else
            {
                newsub.Heading = head;
                DB.SaveChanges();
            }
        }
        public void AddQuestion2Exam(string e, string q, int deg)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == e);
            var myquestion = DB.Questions.FirstOrDefault(ex => ex.Id.ToString() == q);
            if (myexam.Exambody().Contains(myquestion))
            {
                foreach (var eq in myexam.Questions)
                {
                    if (eq.Question.QuestionBody == myquestion.QuestionBody)
                    {
                        eq.Degree = deg;
                    }
                }
            }
            else
            {
                var neweq = new ExamQuestion { Id = System.Guid.NewGuid(), Degree = deg, Exam = myexam, Question = myquestion };
                DB.ExamQuestions.Add(neweq);
                DB.SaveChanges();
                myexam.Questions.Add(neweq);
                DB.SaveChanges();
            }
        }
        public bool RemoveQuestion2Exam(string e, string q)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == e);
            var myquestion = myexam.Questions.FirstOrDefault(ex => ex.Id.ToString() == q);
            DB.ExamQuestions.Remove(myquestion);
            try
            {
                DB.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void AddQuestion2Sub(string e, string sub, string q, int deg)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == e);
            var myquestion = DB.Questions.FirstOrDefault(ex => ex.Id.ToString() == q);
            var mysub = DB.SubQuestions.FirstOrDefault(ex => ex.Id.ToString() == sub);
            if (mysub.Exambody().Contains(myquestion))
            {
                foreach (var eq in myexam.Questions)
                {
                    if (eq.Question.QuestionBody == myquestion.QuestionBody)
                    {
                        eq.Degree = deg;
                    }
                }
            }
            else
            {
                var neweq = new ExamQuestion { Id = System.Guid.NewGuid(), Degree = deg, Question = myquestion };
                DB.ExamQuestions.Add(neweq);
                DB.SaveChanges();
                mysub.Questions.Add(neweq);
                DB.SaveChanges();
            }
        }
        public void RemoveQuestion2Sub(string sub, string q)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var mysub = DB.SubQuestions.FirstOrDefault(ex => ex.Id.ToString() == sub);
            DB.ExamQuestions.Remove(mysub.Questions.FirstOrDefault(ex => ex.Id.ToString() == q));
            DB.SaveChanges();
        }
        public void RemoveSub2Exam(string e, string id)
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var myexam = DB.Exams.FirstOrDefault(ex => ex.Id.ToString() == e);
            myexam.SubQuestions.Remove(myexam.SubQuestions.FirstOrDefault(sub => sub.Id.ToString() == id));
            DB.SaveChanges();
        }
        public void RemoveSub2Sub(string sub, string id)
        {
            try
            {
                DB = new DB();
                UserStore = new UserStore<AspNetUsers>(DB);
                userManager = new UserManager<AspNetUsers>(UserStore);
                var mysub = DB.SubQuestions.FirstOrDefault(ex => ex.Id.ToString() == sub);
                mysub.SubQuestions.Remove(mysub.SubQuestions.FirstOrDefault(ex => ex.Id.ToString() == id));
                DB.SaveChanges();
            }
            catch { }
        }
    }
}