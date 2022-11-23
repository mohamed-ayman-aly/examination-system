using examination_system.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
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
        public string Search(string SearchName = "")
        {
            DB = new DB();
            var Users = DB.Classes.Where(u => u.Name.Contains(SearchName)).ToList();
            string ret = "";
            foreach (var u in Users)
            {
                ret += PartialView("_Class", u).RenderToString();
            }
            return ret;
        }
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
        [Authorize(Roles = "admin,superadmin,professor")]
        public ActionResult Index()
        {
            DB = new DB();
            return View(DB.Classes.ToList());
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