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
                        double notSumbittedTime = 0;
                        double sumbittedTime = 0;
                        double acceptedTime = 0;
                        var myProjectEntry = new MyProjectModelView();
                        myProjectEntry.projectCode = project.code;
                        myProjectEntry.projectName = project.name;
                        myProjectEntry.manager = user.userName;
                        myProjectEntry.active = project.active;
                        db.Entry(project).Collection(d => d.entries).Load();
                        db.Entry(project).Collection(d => d.accepted).Load();
                        foreach (var raport in project.entries)
                        {
                            if (db.Raport.Find(raport.raportId).frozen == false)
                            {
                                notSumbittedTime += raport.time;
                            }
                            else
                            {
                                sumbittedTime += raport.time;
                            }
                        }
                        foreach (var acc in project.accepted)
                        {
                            acceptedTime += acc.time;
                        }
                        myProjectEntry.notSubmittedHours = Math.Round(notSumbittedTime / 60, 2);
                        myProjectEntry.submittedHours = Math.Round(sumbittedTime / 60, 2);
                        myProjectEntry.acceptedHours = Math.Round(acceptedTime / 60, 2);
                        myProjectEntry.startbudget = project.budget / 60;
                        myProjectEntry.budgetNow = Math.Round((myProjectEntry.startbudget - myProjectEntry.acceptedHours));

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
            var userName = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["User"] = userName;
            ViewData["ProjectCode"] = projectCode;

            if (ViewData["User"] != null)
            {
                var returnList = new List<ProjectUserRaportModelView>();
                using (var db = new RaportContext())
                {
                    var findProject = db.Project.Where(d => d.code == projectCode).Single();
                    var allUsers = db.User.AsEnumerable().ToList();

                    foreach (var user in allUsers)
                    {
                        var userRaports = db.Raport.Where(d => d.userId == user.userId).ToList();
                        foreach (var raport in userRaports)
                        {
                            
                            var userEntries = db.RaportEntry.Where(d => d.raportId == raport.raportId && d.activityId == findProject.activityId).ToList();
                            var userAccepted = db.AcceptedTime.Where(d => d.raportId == raport.raportId && d.activityId == findProject.activityId).ToList();

                            if(userEntries.Count()+userAccepted.Count() > 0){
                                var forViewData = new ProjectUserRaportModelView();
                                // project info
                                forViewData.projectCode = findProject.code;
                                forViewData.projectActive = findProject.active;
                                // user info
                                forViewData.userName = user.userName;
                                forViewData.name = user.name;
                                forViewData.surname = user.surname;
                                // raport info TODO

                                forViewData.period = raport.year.ToString() + '-' + raport.month.ToString();
                                if (userAccepted.Count() > 0)
                                {
                                    forViewData.accepted = true;
                                    forViewData.timeAccepted = userAccepted.Where(e => e.activityId == findProject.activityId).Select(d => d.time).Sum();

                                }
                                else
                                {
                                    forViewData.accepted = false;
                                    forViewData.timeAccepted = 0;
                                }
                                forViewData.timeSubmitted = userEntries.Where(e => e.activityId == findProject.activityId).Select(d => d.time).Sum();
                                forViewData.frozen = raport.frozen;
                                forViewData.raportId = raport.raportId;
                                returnList.Add(forViewData);
                            }
                            

                        }

                    }
                    return View(returnList);

                }
            }
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
                        try{
                            db.Entry(project).OriginalValues["TimeStamp"] = a.TimeStamp;
                            db.SaveChanges();
                        }
                        catch(Microsoft.EntityFrameworkCore.DbUpdateException err){
                            ViewData["Error"] = true;
                            ViewData["Operacja"] = "Edycja";
                            ViewData["ErrorMessage"] = "Edycja danego projektu nie udała się - w między czasie ktoś edytował ten wpis."
                                                    + "System zabezpiecza przed edycją danych których użykownik nie widział.";
                            return View("ConcurrencyError");
                            }
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
                        if(db.Project.Where(d => d.code == a.code).Count()!= 0){
                            return View("ConcurrencyError");
                        }
                        a.active = true;
                        a.budget = a.budget * 60;
                        a.managerId = db.User.Where(u => u.userName == userName).Select(u => u.userId).Single();
                        db.Project.Add(a);
                        db.SaveChanges();

                    }
                    return RedirectToAction("MyProjects", "Projects");
                }
                else
                {
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");


        }
        public IActionResult Details(int id, string projectCode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                using (var db = new RaportContext())
                {
                    var raport = db.Raport.Find(id);
                    var project = db.Project.Where(d => d.code == projectCode).Single();
                    db.Entry(raport).Collection(d => d.entries).Load();
                    var raportEntires = new List<Entry>();
                    foreach (var entry in raport.entries)
                    {
                        if (entry.activityId == project.activityId)
                        {
                            entry.Subactivity = db.Subactivity.Find(entry.subactivityId);
                            raportEntires.Add(entry);
                        }
                    }
                    return View(raportEntires);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Accept(int id, string acceptedTime, string projectCode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                using (var db = new RaportContext())
                {
                    var acceptedRaport = db.Raport.Find(id);
                    var acceptedProject = db.Project.Where(d => d.code == projectCode).Single();

                    if(db.AcceptedTime.Where(d => d.activityId == acceptedProject.activityId && d.raportId == acceptedRaport.raportId).Count()!=0){
                        ViewData["Error"] = true;
                        ViewData["Operacja"] = "Akceptacja wpisów";
                        ViewData["ErrorMessage"] = "Kto akceptował już ten wpis przed tobą..";
                        return View("ConcurrencyError");
                    }
                    var newAccepted = new Accepted();
                    newAccepted.Activity = acceptedProject;
                    newAccepted.activityId = acceptedProject.activityId;
                    newAccepted.Raport = acceptedRaport;
                    newAccepted.raportId = acceptedRaport.raportId;
                    newAccepted.time = Int32.Parse(acceptedTime);
                    db.AcceptedTime.Add(newAccepted);
                    db.SaveChanges();

                    return RedirectToAction("Manage", "Projects", new { projectCode = projectCode });
                }

            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Change(int id, string acceptedTime, string oldAcceptedTime, string projectCode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                using (var db = new RaportContext())
                {
                    var acceptedRaport = db.Raport.Find(id);
                    var acceptedProject = db.Project.Where(d => d.code == projectCode).Single();
                    var acceptedActept = db.AcceptedTime.Where(d => d.activityId == acceptedProject.activityId && d.raportId == acceptedRaport.raportId).Single();
                    if(acceptedActept.time!=  Int32.Parse(oldAcceptedTime))
                    {
                        ViewData["Error"] = true;
                        ViewData["Operacja"] = "Zmiana zakaceptowanego czasu";
                        ViewData["ErrorMessage"] = "Edycja danego rekordu nie udała się - w między czasie ktoś edytował ten wpis."
                                                  + "System zabezpiecza przed edycją danych których użykownik nie widział.";
                        return View("ConcurrencyError");
                    }
                    acceptedActept.time = Int32.Parse(acceptedTime);
                    db.SaveChanges();
                }
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