using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace TimeReportingSystem.Controllers
{
    public class ProjectsController : Controller
    {

        private RaportContext _context;

        public IActionResult Index()
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                using (var db = new RaportContext())
                {
                    var allProjects = db.Project.ToList();

                    foreach (var item in allProjects)
                    {
                        item.manager = db.User.Where(u => u.userId == item.managerId).ToList()[0];
                        db.Entry(item).Collection(x => x.subactivities).Load();
                    }
                    return View(allProjects);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult MyProjects()
        {
            var userName = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["User"] = userName;

            if (ViewData["User"] != null)
            {

                using (var db = new RaportContext())
                {
                    var user = db.User.Where(u => u.userName == userName).Single();
                    var userId = user.userId;
                    var allMyProjects = db.Project.Where(u => u.managerId == userId).ToList();
                    var myProjects = new List<MyProjectModelView>();
                    foreach (var project in allMyProjects.OrderByDescending(f => f.active))
                    {
                        var myProjectEntry = new MyProjectModelView();
                        myProjectEntry.projectCode = project.code;
                        myProjectEntry.projectName = project.name;
                        myProjectEntry.manager = user.userName;
                        myProjectEntry.active = project.active;

                        // TODO hours

                        myProjectEntry.notSubmittedHours = 0;
                        myProjectEntry.submittedHours = 0;
                        myProjectEntry.acceptedHours = 0;
                        myProjectEntry.budgetNow = 0;
                        myProjectEntry.startbudget = project.budget / 60;

                        var subAct = new List<string>();
                        db.Entry(project).Collection(x => x.subactivities).Load();
                        foreach (var subactivity in project.subactivities)
                        {
                            subAct.Add(subactivity.code);
                        }
                        myProjectEntry.subactivituNames = subAct;
                        myProjects.Add(myProjectEntry);


                    }
                    return View(myProjects);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Manage(string projectCode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["ProjectCode"] = projectCode;

            // if(ViewData["User"] != null){
            //     var manageData = appRepository.GetManageData(projectCode); 
            //     return View(manageData);
            // }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Edit(string projectCode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["ProjectCode"] = projectCode;

            if (ViewData["User"] != null)
            {
                using (var db = new RaportContext())
                {
                    var project = db.Project.Where(u => u.code == projectCode).Single();
                    return View(project);
                }

            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Edit(TimeReportingSystem.Models.Activity a)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                if (ModelState.IsValid)
                {
                    using (var db = new RaportContext())
                    {
                        var project = db.Project.Where(u => u.code == a.code).Single();
                        project.name = a.name;
                        project.budget = a.budget * 60;
                        db.SaveChanges();
                        return RedirectToAction("MyProjects", "Projects");
                    }

                }
                else
                {
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NewSubactivity(string projectCode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["ProjectCode"] = projectCode;

            if (ViewData["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult NewSubactivity(Subactivity s, string projectCode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {

                var userName = ViewData["User"].ToString();

                using (var db = new RaportContext())
                {
                    var user = db.User.Where(u => u.userName == userName).Single();
                    var userId = user.userId;
                    var projectForSub = db.Project.Where(u => u.code == projectCode).Single();
                    db.Entry(projectForSub).Collection(x => x.subactivities).Load();
                    if (projectForSub.managerId == userId)
                    {
                        if (ModelState.IsValid)
                        {
                            projectForSub.subactivities.Add(s);
                            db.SaveChanges();
                            return RedirectToAction("MyProjects", "Projects");
                        }
                        else
                        {
                            return View();
                        }
                    }
                }
                // var projectInfo = appRepository.GetProjectInfo(projectCode);
                // if(projectInfo.manager == userName)
                // {
                //     if(ModelState.IsValid){
                //         // appRepository.InsertSubActivity(s, projectCode);
                //         return RedirectToAction("MyProjects", "Projects");
                //     }
                //     else{
                //         return View();
                //     }
                // }
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult CloseProject(string projectCode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                var userName = ViewData["User"].ToString();

                using (var db = new RaportContext())
                {
                    var user = db.User.Where(u => u.userName == userName).Single();
                    var userId = user.userId;
                    var projectToClose = db.Project.Where(u => u.code == projectCode).Single();
                    if (projectToClose.active && projectToClose.managerId == userId)
                    {
                        projectToClose.active = false;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("MyProjects", "Projects");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateProject()
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult CreateProject(TimeReportingSystem.Models.Activity a)
        {
            var userName = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["User"] = userName;

            if (ViewData["User"] != null)
            {

                if (ModelState.IsValid)
                {
                    using (var db = new RaportContext())
                    {

                        // var newProject = new TimeReportingSystem.Models.Activity();
                        a.active = true;
                        a.budget = a.budget * 60;
                        a.managerId = db.User.Where(u => u.userName == userName).Select(u => u.userId).Single();
                        db.Project.Add(a);
                        db.SaveChanges();

                    }
                    // appRepository.InsertActivity(a);
                    return RedirectToAction("MyProjects", "Projects");
                }
                else
                {
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");


        }
        public IActionResult Details(string userName, string projectCode, string period)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                var year = period.Substring(0, 4);
                var month = period.Substring(5, 2);
                ViewData["SelectedUser"] = userName;
                ViewData["ProjectCode"] = projectCode;
                ViewData["Period"] = period;
                // ViewData["Users"] = appRepository.GetUsersData();
                // return View(appRepository.GetUserRaport(userName, year, month));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Accept(string userName, string projectCode, string period, string acceptedTime)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                var year = period.Substring(0, 4);
                var month = period.Substring(5, 2);
                var managerName = ViewData["User"].ToString();
                // if(appRepository.GetProjectInfo(projectCode).manager == managerName){
                //     appRepository.AcceptRaport(year, month, userName, projectCode, Int32.Parse(acceptedTime));
                //     return RedirectToAction("Manage", "Projects", new{projectCode = projectCode});
                // }

            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Change(string userName, string projectCode, string period, string acceptedTime)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                var year = period.Substring(0, 4);
                var month = period.Substring(5, 2);
                // appRepository.ChangeRaport(year, month, userName, projectCode, Int32.Parse(acceptedTime));
                return RedirectToAction("Manage", "Projects", new { projectCode = projectCode });

            }
            return RedirectToAction("Index", "Home");
        }

        public JsonResult ProjCodeUniqueness(string code)
        {
            if (!ProjectCodeIsInUse(code))
            {
                return Json($"Istnieje już projekt o podanym kodzie!");
            }
            return Json(true);
        }
        public bool ProjectCodeIsInUse(string projectCode)
        {
            bool isNotTaken = true;
            using (var db = new RaportContext())
            {
                db.Project.ToList().ForEach(delegate (TimeReportingSystem.Models.Activity a) { if (a.code == projectCode) { isNotTaken = false; } });
            }
            return isNotTaken;
        }
        public JsonResult SubCodeUniqueness(string code, string projectCode)
        {
            if (!SubActivityCodeIsInUse(code, projectCode))
            {
                return Json($"Kod {code} w danym projekcie już istnieje.");
            }
            return Json(true);
        }

        public bool SubActivityCodeIsInUse(string code, string projectCode)
        {
            bool isNotTaken = true;
            using (var db = new RaportContext())
            {
                var project = db.Project.Where(u => u.code == projectCode).Single();
                db.Entry(project).Collection(x => x.subactivities).Load();
                foreach (var sub in project.subactivities)
                {
                    if (sub.code == code)
                    {
                        isNotTaken = false;
                    }
                }
            }

            return isNotTaken;
        }
    }
}