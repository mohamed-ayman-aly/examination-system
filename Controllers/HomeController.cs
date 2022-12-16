using examination_system.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Controllers
{
    public class HomeController : Controller
    {
        static DB DB = new DB();
        static UserStore<AspNetUsers> UserStore = new UserStore<AspNetUsers>(DB);
        static UserManager<AspNetUsers> userManager = new UserManager<AspNetUsers>(UserStore);
        [AllowAnonymous]
        public ActionResult Index()
        {
            DB = new DB();
            UserStore = new UserStore<AspNetUsers>(DB);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var userid = User.Identity.GetUserId();
            if (userid != null)
            {
                var Classes = DB.Users.FirstOrDefault(u => u.Id == userid).Classes.ToList();
                List<Exam> Exams = new List<Exam>();
                foreach (var Class in Classes)
                {
                    var classexams = Class.Exams.Where(e => e.Date.AddMinutes(e.Duration) > DateTime.Now).ToList();
                    Exams.AddRange(classexams);
                }
                return View(Exams);
            }
            return View();
        }
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}