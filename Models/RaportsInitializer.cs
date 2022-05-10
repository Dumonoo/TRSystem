using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// using System.Data.Entity;
using TimeReportingSystem.Models;

namespace TimeReportingSystem.Models
{
    public class RaportsInitializer
    {
        public void Seed()
        {
            var users = new List<User>{
                new User{userId=1, userName="jarooo12", name="Jacek", surname="Kawka"},
                new User{userId=2, userName="test", name="Testname", surname="TestSurname"},
                new User{userId=3, userName="dumono", name="Dominik", surname="Nowak"},
                new User{userId=4, userName="jkowalski", name="Jan", surname="Kowalski"},
                new User{userId=5, userName="rmaklow2", name="Rob", surname="Maklowicz"},
            };
            using (var db = new RaportContext())
            {
                users.ForEach(u => db.User.Add(u));
                db.SaveChanges();
            }
            var projects = new List<Activity>{
                new Activity{activityId=1, managerId=1, code="yang", budget=12000, active=true, name="Super Yang"},
                new Activity{activityId=2, managerId=1, code="yang2", budget=1234, active=false, name="Super Yang 2"},
                new Activity{activityId=3, managerId=2, code="cyber", budget=131233, active=true, name="Cyber bug 3"},
                new Activity{activityId=4, managerId=2, code="hades", budget=1234, active=true, name="Hades 2"},
                new Activity{activityId=5, managerId=2, code="ntrlab", budget=51234, active=false, name="Gang plank"},
                new Activity{activityId=6, managerId=2, code="cang", budget=41234, active=true, name="Test test"},
                new Activity{activityId=7, managerId=5, code="dang", budget=31234, active=true, name="Hobbit"},
            };
            using (var db = new RaportContext())
            {
                projects.ForEach(p => db.Project.Add(p));
                db.SaveChanges();
            }
            var subActivities = new List<Subactivity>{
                new Subactivity{subactivityId=1, code="coding", activityId=1},
                new Subactivity{subactivityId=2, code="testing", activityId=1},
                new Subactivity{subactivityId=3, code="coding", activityId=3},
                new Subactivity{subactivityId=4, code="testing", activityId=3},
            };
            using (var db = new RaportContext())
            {
                subActivities.ForEach(p => db.Subactivity.Add(p));
                db.SaveChanges();
            }
            var raports = new List<Raport>{
                new Raport{raportId=1, frozen=false, year=2021, month=12, userId=1},
                new Raport{raportId=2, frozen=true, year=2021, month=12, userId=2},
                new Raport{raportId=3, frozen=false, year=2021, month=12, userId=3},
                new Raport{raportId=4, frozen=true, year=2021, month=12, userId=4},
            };
            using (var db = new RaportContext())
            {
                raports.ForEach(p => db.Raport.Add(p));
                db.SaveChanges();
            }
            var entries = new List<Entry>{
                new Entry{entryId=1, date="2012-12-01", time=120, description="example descryption", activityId=1, raportId=1},
                new Entry{entryId=2, date="2012-12-01", time=180, description="example descryption", activityId=2, raportId=1},
                new Entry{entryId=3, date="2012-12-01", time=1200, description="example descryption", activityId=1, raportId=2},
                new Entry{entryId=4, date="2012-12-01", time=1320, description="example descryption", activityId=3, raportId=2},
                new Entry{entryId=5, date="2012-12-01", time=2120, description="example descryption", activityId=4, raportId=2},
                new Entry{entryId=6, date="2012-12-01", time=5120, description="example descryption", activityId=5, raportId=3},
            };
            using (var db = new RaportContext())
            {
                entries.ForEach(p => db.RaportEntry.Add(p));
                db.SaveChanges();
            }
        }

    }

}