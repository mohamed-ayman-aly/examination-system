using examination_system.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Controllers
{
    public class ClassController : Controller
    {
        // GET: Class
        static DB DB = new DB();
        [Authorize(Roles = "admin,superadmin")]
        public bool Delete(string Id)
        {
            DB = new DB();
            Class c = DB.Classes.FirstOrDefault(u => u.Id.ToString() == Id);
            DB.Classes.Remove(c);
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
        const int pageSize = 29;
        [Authorize(Roles = "admin,superadmin,professor")]
        public ActionResult Index(string SearchName = "", int pg = 1)
        {
            DB = new DB();
            var Classes = DB.Classes.Where(u => u.Name.Contains(SearchName));
            int recsCount = Classes.Count();
            if (pg < 1)
                pg = 1;
            if (recsCount != 0 && pg > (int)Math.Ceiling((decimal)recsCount / pageSize))
            {
                pg = (int)Math.Ceiling((decimal)recsCount / pageSize);
            }
            Pager pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = Classes.OrderBy(e=>e.Name).Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            this.ViewBag.SearchName = SearchName;

            return View(data);
        }
        [Authorize(Roles = "student,superadmin")]
        public ActionResult Register()
        {
            DB = new DB();
            string userid = User.Identity.GetUserId();
            var aspNetUsers = DB.Users.FirstOrDefault(u => u.Id == userid);
            var myClasses = aspNetUsers.Classes.ToList();
            var myClassesn = aspNetUsers.Classes.Select(c => c.Name).ToList();
            var OtherClasses= DB.Classes.OrderBy(u => u.Name).ToList();
            for (int i=0;i< OtherClasses.Count();i++) {
                if (myClassesn.Contains(OtherClasses[i].Name)) {
                    OtherClasses.RemoveAt(i);
                    i--;
                }
            }
            ViewBag.myClasses = myClasses;
            ViewBag.OtherClasses = OtherClasses;
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "student,superadmin")]
        public ActionResult DeleteRegister(string Id) {
            DB = new DB();
            string userid = User.Identity.GetUserId();
            var aspNetUsers = DB.Users.FirstOrDefault(u => u.Id == userid);
            var myClass = aspNetUsers.Classes.FirstOrDefault(c=>c.Id==new Guid(Id));
            aspNetUsers.Classes.Remove(myClass);
            DB.SaveChanges();
            return RedirectToAction("Register");
        }
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "student,superadmin")]
        public ActionResult Register(string Class)
        {
            DB = new DB();
            string userid = User.Identity.GetUserId();
            var NewClass =DB.Classes.FirstOrDefault(c => c.Id.ToString() == Class);
            var user = DB.Users.FirstOrDefault(u => u.Id == userid);
            NewClass.Students.Add(user);
            DB.SaveChanges();
            return RedirectToAction("Register");
        }
        [Authorize(Roles ="admin,superadmin")]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "admin,superadmin")]
        public ActionResult Add(Class Newclass)
        {
            DB = new DB();
            Newclass.Id = Guid.NewGuid();
            if (!ModelState.IsValid)
                return View(Newclass);
            DB.Classes.Add(Newclass);
            DB.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "admin,superadmin")]
        public ActionResult Edit(string Id)
        {
            DB = new DB();
            var Class=DB.Classes.FirstOrDefault(c => c.Id.ToString() == Id);
            return View(Class);
        }
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "admin,superadmin")]
        public ActionResult Edit(Class Editedclass)
        {
            DB = new DB();
            if (!ModelState.IsValid)
                return View(Editedclass);
            var Class = DB.Classes.FirstOrDefault(c => c.Id.ToString() == Editedclass.Id.ToString());
            Class.Name= Editedclass.Name;
            DB.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}