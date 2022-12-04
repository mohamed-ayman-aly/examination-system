using examination_system.Models;
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
        [HttpPost,ValidateAntiForgeryToken,Authorize(Roles = "student")]
        public ActionResult Index(string id)
        {
            DB = new DB();

            return View();
        }
    }
}