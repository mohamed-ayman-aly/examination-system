using examination_system.Models;
using examination_system.Models.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Controllers
{
    [Authorize(Roles = "professor,superadmin")]
    public class QuestionController : Controller
    {
        static DB DB = new DB();
        public string Search(string SearchName = "")
        {
            DB = new DB();
            var Users = DB.Questions.Where(q => q.QuestionBody.Contains(SearchName)).ToList();
            string ret = "";
            foreach (var u in Users)
            {
                ret += PartialView("_Question", u).RenderToString();
            }
            return ret;
        }
        public string SearchClass(string SearchName = "")
        {
            DB = new DB();
            var Users = DB.Questions.Where(q => q.Class.Name.Contains(SearchName)).ToList();
            string ret = "";
            foreach (var u in Users)
            {
                ret += PartialView("_Question", u).RenderToString();
            }
            return ret;
        }
        public bool Delete(string Id)
        {
            DB = new DB();
            var q = DB.Questions.FirstOrDefault(u => u.Id.ToString() == Id);
            DB.Answers.RemoveRange(q.Answers.ToList());
            try
            {
                DB.SaveChanges();
            }
            catch
            {
                return false;
            }
            DB.Questions.Remove(q);
            try
            {
                DB.SaveChanges();
                return true;
            }
            catch {
                return false;
            }
        }
        public ActionResult Index()
        {
            DB = new DB();
            return View(DB.Questions.ToList());
        }
        public ActionResult Add()
        {
            DB = new DB();
            ViewBag.Class = DB.Classes.ToList();
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(Question NewQuestion,string CorrectAnswer, List<string> Answers,string Class)
        {
            DB = new DB();
            // i know that i am supposed to make a modelview for validation but i am bored and lazy😜😜😜
            if (CorrectAnswer == null|| CorrectAnswer =="") {
                ModelState.AddModelError("", "Correct Answer is Required");
                ViewBag.CorrectAnswer = CorrectAnswer;
                ViewBag.Answers = Answers;
                ViewBag.Class = DB.Classes.ToList();
                return View(NewQuestion);
            }
            if (Answers == null || Answers.Count == 0) {
                ModelState.AddModelError("", "new Questions need more than one Answer");
                ViewBag.CorrectAnswer = CorrectAnswer;
                ViewBag.Answers = Answers;
                ViewBag.Class = DB.Classes.ToList();
                return View(NewQuestion);
            }
            foreach (var a in Answers) {
                if (a == CorrectAnswer) {
                    ModelState.AddModelError("", "Correct Answer should not repeat");
                    ViewBag.CorrectAnswer = CorrectAnswer;
                    ViewBag.Answers = Answers;
                    ViewBag.Class = DB.Classes.ToList();
                    return View(NewQuestion);
                }
                if (a == null || a == "") {
                    ModelState.AddModelError("", "can't add blank'empty' Answer");
                    ViewBag.CorrectAnswer = CorrectAnswer;
                    ViewBag.Answers = Answers;
                    ViewBag.Class = DB.Classes.ToList();
                    return View(NewQuestion);
                }
            }
            if (NewQuestion == null || NewQuestion.QuestionBody == "") {
                ModelState.AddModelError("", "can't add blank'empty' Question");
                ViewBag.CorrectAnswer = CorrectAnswer;
                ViewBag.Answers = Answers;
                ViewBag.Class = DB.Classes.ToList();
                return View(NewQuestion);
            }
            if (Class == null || Class == "")
            {
                ModelState.AddModelError("", "Correct Answer is Required");
                ViewBag.CorrectAnswer = CorrectAnswer;
                ViewBag.Answers = Answers;
                ViewBag.Class = DB.Classes.ToList();
                return View(NewQuestion);
            }

            NewQuestion.Id = Guid.NewGuid();
            Answer Correct= new Answer { AnswerBody = CorrectAnswer, Id = Guid.NewGuid() };
            DB.Answers.Add(Correct);
            DB.SaveChanges();
            NewQuestion.Answers = new List<Answer>();
            foreach (var a in Answers)
            {
                NewQuestion.Answers.Add( new Answer { AnswerBody = a, Id = Guid.NewGuid() });
            }
            var myClass = DB.Classes.FirstOrDefault(c => c.Id.ToString() == Class);
            NewQuestion.CorrectAnswer = Correct;
            NewQuestion.Answers.Add(Correct);
            myClass.Questions.Add(NewQuestion);
            DB.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(string Id)
        {
            DB = new DB();
            var Question = DB.Questions.FirstOrDefault(c => c.Id.ToString() == Id);
            ViewBag.Class = DB.Classes.ToList();
            ViewBag.Answers = Question.Answers;
            return View(Question);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Question NewQuestion, List<string> Answers, string Class)
        {
            DB = new DB();
            // i know that i am supposed to make a modelview for validation but i am bored and lazy 😜😜😜
            var q = DB.Questions.FirstOrDefault(qu => qu.Id == NewQuestion.Id);
            if (NewQuestion.CorrectAnswer == null || NewQuestion.CorrectAnswer.AnswerBody == "")
            {
                ModelState.AddModelError("", "Correct Answer is Required");
                ViewBag.Answers = Answers;
                ViewBag.Class = DB.Classes.ToList();
                return View(q);
            }
            if (Answers == null || Answers.Count == 0)
            {
                ModelState.AddModelError("", "new Questions need more than one Answer");
                ViewBag.Answers = Answers;
                ViewBag.Class = DB.Classes.ToList();
                return View(NewQuestion);
            }
            foreach (var a in Answers)
            {
                if (a == NewQuestion.CorrectAnswer.AnswerBody)
                {
                    ModelState.AddModelError("", "Correct Answer should not repeat");
                    ViewBag.Answers = Answers;
                    ViewBag.Class = DB.Classes.ToList();
                    return View(NewQuestion);
                }
                if (a == null || a == "")
                {
                    ModelState.AddModelError("", "can't add blank'empty' Answer");
                    ViewBag.Answers = Answers;
                    ViewBag.Class = DB.Classes.ToList();
                    return View(NewQuestion);
                }
            }
            if (NewQuestion == null || NewQuestion.QuestionBody == "")
            {
                ModelState.AddModelError("", "can't add blank'empty' Question");
                ViewBag.Answers = Answers;
                ViewBag.Class = DB.Classes.ToList();
                return View(NewQuestion);
            }
            if (Class == null || Class == "")
            {
                ModelState.AddModelError("", "Correct Answer is Required");
                ViewBag.Answers = Answers;
                ViewBag.Class = DB.Classes.ToList();
                return View(NewQuestion);
            }
            q.QuestionBody = NewQuestion.QuestionBody;
            q.Class=DB.Classes.FirstOrDefault(c=>c.Id.ToString()==Class);
            q.CorrectAnswer.AnswerBody=NewQuestion.CorrectAnswer.AnswerBody;
            DB.SaveChanges();
            if (Answers.Count >= q.Answers.Count-1)
            {
                for (int i = 0; i < q.Answers.Count; i++)
                {
                    if (q.Answers[i].AnswerBody!= NewQuestion.CorrectAnswer.AnswerBody) {
                        q.Answers[i].AnswerBody = Answers[0];
                        Answers.RemoveAt(0);
                        continue;
                    }
                }
                foreach (var a in Answers)
                {
                    q.Answers.Add(new Answer { Id = Guid.NewGuid(), AnswerBody = a });
                }
            }
            else
            {
                int outofindex = 0;
                for (int i=0;i< q.Answers.Count- Answers.Count; i++)
                {
                    if (q.Answers[i+outofindex].AnswerBody != NewQuestion.CorrectAnswer.AnswerBody)
                    {
                        DB.Answers.Remove(q.Answers[i + outofindex]);
                        continue;
                    }
                    outofindex++;
                    i--;
                }
                for (int i = 0; i < q.Answers.Count; i++)
                {
                    if (q.Answers[i].AnswerBody != NewQuestion.CorrectAnswer.AnswerBody)
                    {
                        q.Answers[i].AnswerBody = Answers[0];
                        Answers.RemoveAt(0);
                        continue;
                    }
                }
            }
            DB.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}