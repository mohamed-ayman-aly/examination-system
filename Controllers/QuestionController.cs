using examination_system.Models;
using examination_system.Models.View_Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Controllers
{
    [Authorize(Roles = "professor,superadmin")]
    public class QuestionController : Controller
    {
        static DB DB = new DB();
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
        const int pageSize = 9;
        public ActionResult Index(string SearchName = "", string Searchclass = "", int pg = 1)
        {
            DB = new DB();
            var Questions = DB.Questions.Where(q => q.QuestionBody.Contains(SearchName)
            && q.Class.Name.Contains(Searchclass));
            int recsCount = Questions.Count();
            if (pg < 1)
                pg = 1;
            if (recsCount != 0 && pg > (int)Math.Ceiling((decimal)recsCount / pageSize))
            {
                pg = (int)Math.Ceiling((decimal)recsCount / pageSize);
            }
            Pager pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = Questions.OrderBy(q => q.QuestionBody).Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            this.ViewBag.SearchName = SearchName;
            this.ViewBag.Searchclass = Searchclass;
            return View(data);
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
            var deletedAnswers = q.Answers.Where(a=>a.Id!=q.CorrectAnswer.Id && !Answers.Contains(a.AnswerBody)).ToList();
            q.Answers.RemoveAll(a => a.Id != q.CorrectAnswer.Id && !Answers.Contains(a.AnswerBody));
            var ans= q.Answers.Where(a => a.Id != q.CorrectAnswer.Id).Select(a => a.AnswerBody).ToList();
            var addedAnswers = Answers.Where(adans => !ans.Contains(adans)).ToList();
            foreach (var a in addedAnswers)
            {
                q.Answers.Add(new Answer { Id = Guid.NewGuid(), AnswerBody = a });
            }
            DB.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}