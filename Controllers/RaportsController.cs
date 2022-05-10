using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

using System.IO;


namespace TimeReportingSystem.Controllers
{
    public class RaportsController : Controller
    {
        public IActionResult Index(string year, string month, string day)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                ViewData["Raport"] = "false";
                if (year == null || month == null)
                {
                    year = DateTime.Now.Year.ToString();
                    month = DateTime.Now.Month.ToString();
                }
                if (day != null)
                {
                    ViewData["Day"] = day;
                }
                var userName = ViewData["User"].ToString();
                ViewData["Year"] = year;
                ViewData["Month"] = month;
                ViewData["IsSubmitted"] = false;
                ViewData["Hours"] = (double)0;
                ViewData["RaportId"] = 0;
                if (Int32.Parse(month) <= 9)
                {
                    month = "0" + Int32.Parse(month).ToString();
                }
                using (var db = new RaportContext())
                {
                    var activeUser = db.User.Where(u => u.userName == userName).Single();
                    db.Entry(activeUser).Collection(x => x.Raports).Load();
                    var recentRaport = activeUser.Raports.Where(r => r.year == Int32.Parse(year) && r.month == Int32.Parse(month));

                    if (recentRaport.Count() != 0)
                    {
                        var raportRaport = recentRaport.ElementAt(0);
                        db.Entry(raportRaport).Collection(d => d.entries).Load();
                        db.Entry(raportRaport).Collection(d => d.accepted).Load();
                        ViewData["RaportId"] = raportRaport.raportId;
                        foreach (var entry in raportRaport.entries)
                        {
                            entry.Subactivity = db.Subactivity.Find(entry.subactivityId);
                            entry.Activity = db.Project.Find(entry.activityId);
                        }
                        ViewData["Raport"] = "true";
                        ViewData["IsSubmitted"] = raportRaport.frozen;
                        if (day != null)
                        {
                            double minutesSum = 0;
                            var dailyRaport = raportRaport.entries.Where(d => Int32.Parse(d.date.Substring(8, 2)) == Int32.Parse(day));

                            foreach (var entry in raportRaport.entries)
                            {
                                if (Int32.Parse(entry.date.Substring(8, 2)) == Int32.Parse(day))
                                {
                                    minutesSum += entry.time;
                                }
                            }
                            ViewData["Hours"] = minutesSum / 60;
                        }

                        return View(raportRaport.entries);
                    }
                    else
                    {
                        return View();
                    }

                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateEntry(string year, string month)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);


            if (ViewData["User"] != null)
            {
                var userName = ViewData["User"].ToString();
                using (var db = new RaportContext())
                {
                    var activeUser = db.User.Where(u => u.userName == userName).Single();

                    ViewData["Month"] = month;
                    ViewData["Year"] = year;
                    var allProjects = db.Project.ToList();

                    foreach (var item in allProjects)
                    {
                        item.manager = db.User.Where(u => u.userId == item.managerId).ToList()[0];
                        db.Entry(item).Collection(x => x.subactivities).Load();
                    }
                    ViewData["projectsInfo"] = ToDictionary(allProjects);
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult CreateEntry(Entry e, string code, string subcode)
        {

            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                if (subcode == "null" || subcode == null)
                {
                    subcode = "";
                }
                if (e.description == "null" || e.description == null)
                {
                    e.description = "";
                }
                var userName = ViewData["User"].ToString();
                int year = Int32.Parse(e.date.Substring(0, 4));
                int month = Int32.Parse(e.date.Substring(5, 2));
                using (var db = new RaportContext())
                {
                    var activeUser = db.User.Where(u => u.userName == userName).Single();
                    db.Entry(activeUser).Collection(x => x.Raports).Load();
                    var recentRaport2 = activeUser.Raports.Where(r => r.year == year && r.month == month);

                    if (recentRaport2.Count() == 0)
                    {
                        var newRaport = new Raport();
                        newRaport.frozen = false;
                        newRaport.month = month;
                        newRaport.year = year;
                        newRaport.userId = activeUser.userId;
                        db.Raport.Add(newRaport);
                        db.SaveChanges();
                    }
                    var recentRaport = activeUser.Raports.Where(r => r.year == year && r.month == month).Single();

                    if (!recentRaport.frozen)
                    {
                        var newEntry = new Entry();
                        var findActivity = db.Project.Where(d => d.code == code).Single();
                        db.Entry(findActivity).Collection(x => x.subactivities).Load();
                        if (subcode == "")
                        {
                            newEntry.subactivityId = null;
                        }
                        else
                        {
                            var findSubactivity = findActivity.subactivities.Where(d => d.code == subcode).Single();
                            newEntry.subactivityId = findSubactivity.subactivityId;
                        }
                        newEntry.date = e.date;
                        newEntry.description = e.description;
                        newEntry.time = e.time;
                        newEntry.raportId = recentRaport.raportId;
                        newEntry.activityId = findActivity.activityId;
                        db.RaportEntry.Add(newEntry);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Raports", new { month = month, year = year });
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Display(string index)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                var userName = ViewData["User"].ToString();
                using (var db = new RaportContext())
                {
                    try {
                        var entry = db.RaportEntry.Find(Int32.Parse(index));
                        entry.Activity = db.Project.Find(entry.activityId);
                        entry.Subactivity = db.Subactivity.Find(entry.subactivityId);
                        return View(entry);
                    }
                    catch(Exception e){
                        ViewData["Error"] = true;
                        ViewData["Operacja"] = "Pokaż szczególy";
                        ViewData["ErrorMessage"] = "Wpis do którego się odwołujesz nie istnieje.";
                        return View("ConcurrencyError");
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Edit(string index)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {

                var userName = ViewData["User"].ToString();
                using (var db = new RaportContext())
                {
                    try{
                        var entry = db.RaportEntry.Find(Int32.Parse(index));
                        entry.Activity = db.Project.Find(entry.activityId);
                        entry.Subactivity = db.Subactivity.Find(entry.subactivityId);

                        ViewData["Month"] = entry.date.Substring(5, 2);
                        // Console.WriteLine(ViewData["Month"].ToString());
                        ViewData["Year"] = entry.date.Substring(0, 4);

                        var allProjects = db.Project.ToList();

                        foreach (var item in allProjects)
                        {
                            item.manager = db.User.Where(u => u.userId == item.managerId).ToList()[0];
                            db.Entry(item).Collection(x => x.subactivities).Load();
                        }
                        ViewData["projectsInfo"] = ToDictionary(allProjects);
                        ViewData["Entry"] = entry;

                        return View(entry);
                    }
                    catch(Exception e){
                        ViewData["Error"] = true;
                        ViewData["Operacja"] = "Edycja";
                        ViewData["ErrorMessage"] = "Wpis do którego się odwołujesz nie istnieje.";
                        return View("ConcurrencyError");
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Edit(Entry e, string index, string code, string subcode)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if (ViewData["User"] != null)
            {
                if (subcode == "null" || subcode == null)
                {
                    subcode = "";
                }
                if (e.description == "null" || e.description == null)
                {
                    e.description = "";
                }
                using (var db = new RaportContext())
                {
                    var updateEntry = db.RaportEntry.Find(Int32.Parse(index));
                    var findActivity = db.Project.Where(d => d.code == code).Single();
                    db.Entry(findActivity).Collection(x => x.subactivities).Load();
                    if (subcode == "")
                    {
                        updateEntry.subactivityId = null;
                    }
                    else
                    {
                        var findSubactivity = findActivity.subactivities.Where(d => d.code == subcode).Single();
                        updateEntry.subactivityId = findSubactivity.subactivityId;
                    }
                    updateEntry.activityId = findActivity.activityId;
                    updateEntry.date = e.date;
                    updateEntry.description = e.description;
                    updateEntry.time = e.time;
                    try
                    {
                        db.Entry(updateEntry).OriginalValues["TimeStamp"] = e.TimeStamp;
                        db.SaveChanges();
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException err)
                    {
                        ViewData["Error"] = true;
                        ViewData["Operacja"] = "Edycja";
                        ViewData["ErrorMessage"] = "Edycja danego rekordu wpisu do raportu nie udała się - w między czasie ktoś edytował ten wpis."
                                                  + "System zabezpiecza przed edycją danych których użykownik nie widział.";
                        return View("ConcurrencyError");
                    }
                    catch (Exception err)
                    { 
                        return View("ConcurrencyError");
                    }
                    return RedirectToAction("Index", "Raports");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Delete(string index)
        {
            var userName = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["User"] = userName;

            if (ViewData["User"] != null)
            {
                using (var db = new RaportContext())
                {
                    try{
                        var user = db.User.Where(d => d.userName == userName).Single();
                        var entryToDelete = db.RaportEntry.Find(Int32.Parse(index));
                        if(db.Raport.Find(entryToDelete.raportId).userId != user.userId)
                        {
                            return View("ConcurrencyError");
                        }
                        db.RaportEntry.Remove(entryToDelete);
                        db.SaveChanges();
                    }
                    catch(Exception e){
                        ViewData["Error"] = true;
                        ViewData["Operacja"] = "Usunięcie";
                        ViewData["ErrorMessage"] = "Wpis do którego się odwołujesz nie istnieje.";
                        return View("ConcurrencyError");
                    }
                }
                return RedirectToAction("Index", "Raports");
            }
            return RedirectToAction("Index", "Home");
        }

        public static Dictionary<string, List<string>> ToDictionary(List<TimeReportingSystem.Models.Activity> a)
        {

            var codes = new Dictionary<string, List<string>>();
            foreach (var item in a)
            {
                if (item.active == true)
                {
                    var subCodes = new List<string>();
                    foreach (var sub in item.subactivities)
                    {
                        subCodes.Add(sub.code);
                    }
                    codes.Add(item.code, subCodes);
                }
            }
            return codes;
        }
        public IActionResult Submit(int id)
        {
            var userName = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["User"] = userName;

            if (ViewData["User"] != null)
            {
                using (var db = new RaportContext())
                {
                    var user = db.User.Where(d => d.userName == userName).Single();
                    var raport = db.Raport.Find(id);
                    if(raport.userId != user.userId)
                    {
                        return View("ConcurrencyError");
                    }
                    raport.frozen = true;
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Raports");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult MonthSummary(string month, string year)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            if (ViewData["User"] != null)
            {
                bool raport = false;

                if (year == null || month == null)
                {
                    year = DateTime.Now.Year.ToString();
                    month = DateTime.Now.Month.ToString();
                }
                var userName = ViewData["User"].ToString();
                ViewData["Year"] = year;
                ViewData["Month"] = month;

                if (Int32.Parse(month) <= 9)
                {
                    month = "0" + month;
                }
                var yearInt = Int32.Parse(year);
                var monthInt = Int32.Parse(month);

                using (var db = new RaportContext())
                {
                    var user = db.User.Where(x => x.userName == userName).Single();
                    var projects = db.Project.ToList();
                    var entries = from p in db.Project
                                  join Entry e in db.RaportEntry
                                      on p.activityId equals e.activityId
                                  join Raport r in db.Raport
                                      on e.raportId equals r.raportId
                                  join User u in db.User
                                      on r.userId equals u.userId
                                  where u.userId == user.userId && r.year == yearInt && r.month == monthInt
                                  select new { p.code, e.time, r.frozen };

                    var accepted = from p in db.Project
                                   join Accepted e in db.AcceptedTime
                                       on p.activityId equals e.activityId
                                   join Raport r in db.Raport
                                       on e.raportId equals r.raportId
                                   join User u in db.User
                                       on r.userId equals u.userId
                                   where u.userId == user.userId && r.year == yearInt && r.month == monthInt
                                   select new { p.code, e.time };
                    var list = new List<Tuple<string, int, int, bool, bool>>();

                    foreach (var project in projects)
                    {
                        if (entries.ToList().Exists(x => x.code == project.code))
                        {
                            raport = true;
                            var projectCode = project.code;
                            bool isSubmitted = entries.Where(x => x.code == projectCode).Select(d => d.frozen).First();
                            bool isAccepted = accepted.ToList().Exists(x => x.code == projectCode);
                            var submittedTime = entries.Where(x => x.code == projectCode).Select(d => d.time).Sum();
                            var acceptedTime = 0;
                            if (isAccepted)
                            {
                                acceptedTime = accepted.Where(x => x.code == projectCode).Select(d => d.time).Sum();
                            }
                            list.Add(new Tuple<string, int, int, bool, bool>(project.code, submittedTime, acceptedTime, isSubmitted, isAccepted));
                        }
                    }
                    ViewData["Summary"] = list;
                    ViewData["Raport"] = raport;
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }


    }
}