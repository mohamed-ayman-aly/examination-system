using examination_system.Models;
using examination_system.Models.View_Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace examination_system.Controllers
{
    public class AccountController : Controller
    {
         
        static DB Db = new DB();
        static UserStore<AspNetUsers> UserStore = new UserStore<AspNetUsers>(Db);
        static UserManager<AspNetUsers> userManager = new UserManager<AspNetUsers>(UserStore);
        const int pageSize = 29;
        [Authorize(Roles = "superadmin,admin")]
        public ActionResult Index(string SearchName="",int pg=1)
        {
            Db = new DB();
            UserStore = new UserStore<AspNetUsers>(Db);
            userManager = new UserManager<AspNetUsers>(UserStore);

            var Users = Db.Users.Where(u=>u.UserName.Contains(SearchName));

            int recsCount = Users.Count();
            if (pg < 1)
                pg = 1;
            if (recsCount != 0 && pg > (int)Math.Ceiling((decimal) recsCount / pageSize)) {
                pg = (int)Math.Ceiling((decimal) recsCount / pageSize);
            }
            Pager pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = Users.OrderBy(a=>a.UserName).Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            this.ViewBag.SearchName = SearchName;

            return View(data);
        }
        [Authorize(Roles = "superadmin,admin")]
        public ActionResult Add() {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken,Authorize(Roles = "superadmin,admin")]
        public async Task <ActionResult> Add(AspNetUsers NewUser, HttpPostedFileBase ImgFile)
        {
            Db = new DB();
            UserStore = new UserStore<AspNetUsers>(Db);
            userManager = new UserManager<AspNetUsers>(UserStore);
            if (ImgFile == null) {
                ModelState.AddModelError("", "add Image File please");
                return View(NewUser);
            }
            if (User.IsInRole("admin")&& NewUser.UserType== UserType.admin) {
                ModelState.AddModelError("", "admin users can't add admin users");
                return View(NewUser);
            }
            NewUser.ImgFileName = NewUser.Id + ImgFile.FileName.Substring(ImgFile.FileName.LastIndexOf("."));
            if (!ModelState.IsValid)
                return View(NewUser);
            try
            {
                IdentityResult result = userManager.CreateAsync(NewUser, NewUser.PasswordHash).Result;
                if (result.Succeeded)
                {
                    return View(NewUser);
                }
                else {
                    foreach (var e in result.Errors)
                        ModelState.AddModelError("", e);
                    return View(NewUser);
                }
            }
            catch
            {
                NewUser.ConfirmPassword = NewUser.PasswordHash;
                IdentityResult result = await userManager.CreateAsync(NewUser);
                if (result.Succeeded)
                {
                    string path = "~/Resources/Account/imgs/" + NewUser.ImgFileName;
                    ImgFile.SaveAs(Server.MapPath(path));
                    string s = NewUser.UserType.ToString();
                    userManager.AddToRole(NewUser.Id, s);
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var e in result.Errors)
                        ModelState.AddModelError("", e);
                    return View(NewUser);
                }
            }
        }
        [Authorize(Roles = "superadmin,admin")]
        public ActionResult Edit(string id)
        {
            Db = new DB();
            UserStore = new UserStore<AspNetUsers>(Db);
            userManager = new UserManager<AspNetUsers>(UserStore);
            return View(Db.Users.FirstOrDefault(u=>u.Id==id));
        }
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "superadmin,admin")]
        public ActionResult Edit(string id,AspNetUsers NewUser, HttpPostedFileBase ImgFile)
        {
            Db = new DB();
            UserStore = new UserStore<AspNetUsers>(Db);
            userManager = new UserManager<AspNetUsers>(UserStore);
            var user=Db.Users.FirstOrDefault(u => u.Id == id);
            
            if (User.IsInRole("admin") && NewUser.UserType == UserType.admin)
            {
                ModelState.AddModelError("", "admin users can't add admin users");
                return View(id);
            }
            user.UserName = NewUser.UserName;
            user.Email = NewUser.Email;
            if (NewUser.ConfirmPassword != ""&& NewUser.PasswordHash != "")
            {
                user.PasswordHash = HashPassword(NewUser.PasswordHash);
                user.ConfirmPassword = HashPassword(NewUser.ConfirmPassword);
            }
            user.PhoneNumber = NewUser.PhoneNumber;
            user.UserType = NewUser.UserType;
            if (ImgFile != null)
            {
                user.ImgFileName = user.Id + ImgFile.FileName.Substring(ImgFile.FileName.LastIndexOf("."));
                string path = "~/Resources/Account/imgs/" + user.ImgFileName;
                ImgFile.SaveAs(Server.MapPath(path));
            }
            string s = NewUser.UserType.ToString();
            string oldroleid = user.Roles.FirstOrDefault().RoleId;
            string oldrole = Db.Roles.FirstOrDefault(r => r.Id == oldroleid).Name;
            userManager.RemoveFromRole(NewUser.Id, oldrole);
            userManager.AddToRole(NewUser.Id, s);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }
        [Authorize(Roles = "superadmin,admin")]
        public void Delete(string Id)
        {
            Db = new DB();
            UserStore = new UserStore<AspNetUsers>(Db);
            userManager = new UserManager<AspNetUsers>(UserStore);
            AspNetUsers user = Db.Users.FirstOrDefault(u => u.Id == Id);
            try
            {
                int.Parse(user.UserType.ToString());
            }
            catch {
                IdentityResult result = userManager.Delete(user);
                if (result.Succeeded)
                {
                    string[] files = Directory.GetFiles(Server.MapPath("/Resources/Account/imgs"));
                    foreach (string file in files) {
                        if (file.Contains(Id)) {
                            System.IO.File.Delete(file);
                        }
                    }
                }
            }
        }
        [AllowAnonymous]
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken,AllowAnonymous]
        public async Task<ActionResult> SignIn(SignInViewModel NewUser)
        {
            Db = new DB();
            UserStore = new UserStore<AspNetUsers>(Db);
            userManager = new UserManager<AspNetUsers>(UserStore);
            if (!ModelState.IsValid)
                return View(NewUser);
            AspNetUsers user = new AspNetUsers
            {
                UserName = NewUser.Username,
                PasswordHash = NewUser.Password,
                Email = NewUser.Email,
                UserType = UserType.student
            };
            user.ImgFileName = user.Id + NewUser.ImgFile.FileName.Substring(NewUser.ImgFile.FileName.LastIndexOf("."));
            try
            {
                IdentityResult result = await userManager.CreateAsync(user, NewUser.Password);
                return View(NewUser);
            }
            catch
            {
                user.ConfirmPassword = user.PasswordHash;
                IdentityResult result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    string path = "~/Resources/Account/imgs/" + user.ImgFileName;
                    NewUser.ImgFile.SaveAs(Server.MapPath(path));
                    userManager.AddToRole(user.Id, UserType.student.ToString());
                    return RedirectToAction("LogIn");
                }
                else
                {
                    foreach (var e in result.Errors)
                        ModelState.AddModelError("", e);
                    return View(NewUser);
                }
            }
        }
        [AllowAnonymous]
        public ActionResult LogIn()
        {
            if(User.Identity.GetUserId()==null)
                return View();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost, ValidateAntiForgeryToken,AllowAnonymous]
        public async Task<ActionResult> LogIn(LogInViewModel User)
        {
            Db = new DB();
            UserStore = new UserStore<AspNetUsers>(Db);
            userManager = new UserManager<AspNetUsers>(UserStore);
            if (!ModelState.IsValid)
                return View(User);
            SignInManager<AspNetUsers, string> SignInManager = new SignInManager<AspNetUsers, string>(userManager, HttpContext.GetOwinContext().Authentication);
            var res = await SignInManager.PasswordSignInAsync(User.Username, User.Password, User.RememberMe,false);
            if(res== SignInStatus.Success)
                return  RedirectToAction("Index", "Home");
            if(res == SignInStatus.LockedOut)
                return View("Lockout");
            if (res == SignInStatus.Failure)
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View(User);
        }
        public ActionResult ViewProfile(string id) 
        {
            Db = new DB();
            UserStore = new UserStore<AspNetUsers>(Db);
            userManager = new UserManager<AspNetUsers>(UserStore);
            
            string userid=User.Identity.GetUserId();
            if (userid == id) {
                var user = Db.Users.FirstOrDefault(u => u.Id == id);
                return View(user);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult LogOut()
        {
            IAuthenticationManager manager= HttpContext.GetOwinContext().Authentication;
            manager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("LogIn", "Account");
        }
    }
}