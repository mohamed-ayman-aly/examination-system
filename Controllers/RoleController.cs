using examination_system.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Controllers
{
    [Authorize(Roles = "superadmin")]
    public class RoleController : Controller
    {
        // GET: Role
        static DB DB = new DB();
        static readonly RoleStore<IdentityRole> store = new RoleStore<IdentityRole>(DB);
        static readonly RoleManager<IdentityRole> manager = new RoleManager<IdentityRole>(store);
        
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(string name)
        {
            DB = new DB();
            if (name != ""){
                IdentityRole role = new IdentityRole
                {
                    Name = name
                };
                IdentityResult res =await manager.CreateAsync(role);
                if (res.Succeeded)
                {
                    return RedirectToAction("add");
                }
                else { return View(name); }
            }
            return View();
        }
    }
}