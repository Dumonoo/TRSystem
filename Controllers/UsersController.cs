using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;
using Microsoft.AspNetCore.Http;




namespace TimeReportingSystem.Controllers
{
    public class UsersController : Controller
    {
        private RaportContext _context;
        public const string SessionUser = "_User";

        public IActionResult Index()
        {
            using (var db = new RaportContext())
            {
                var user = db.User.OrderBy(i => i.userName).ToList();
                return View(user);
            }
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User u)
        {
            if (ModelState.IsValid)
            {
                using (var db = new RaportContext())
                {
                    var newUser = new User();
                    newUser.name = u.name;
                    newUser.surname = u.surname;
                    newUser.userName = u.userName;
                    db.User.Add(newUser);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Users");
            }
            else
            {
                return View();
            }
        }

        public IActionResult SignIn(string userName)
        {
            HttpContext.Session.SetString(SessionUser, userName);
            ViewData["User"] = HttpContext.Session.GetString(SessionUser);
            return RedirectToAction("Index", "Raports");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove(SessionUser);
            ViewData["User"] = HttpContext.Session.GetString(SessionUser);
            return RedirectToAction("Index", "Home");
        }


        public JsonResult VerifyUserName(string userName)
        {
            bool newUsername = IsInUse(userName);

            if (!newUsername)
            {
                return Json($"Nazwa {userName} jest w użyciu.");
            }
            return Json(true);
        }
        public bool IsInUse(string userName)
        {

            bool isNotTaken = true;
            using (var db = new RaportContext())
            {
                db.User.ToList().ForEach(delegate (User u) { if (u.userName == userName) { isNotTaken = false; } });
            }
            return isNotTaken;
        }
    }
}
