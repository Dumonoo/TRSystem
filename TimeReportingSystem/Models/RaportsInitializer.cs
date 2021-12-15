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
                new User{userId=1, userName="Jarooo12", name="Jacek", surname="Kawka"},
                new User{userId=2, userName="JMyrcha", name="Julian", surname="Myrcha"},
                new User{userId=3, userName="Dumono", name="Dominik", surname="Nowak"},
                new User{userId=4, userName="jkowalski", name="Jan", surname="Kowalski"},
                new User{userId=5, userName="rmaklow2", name="Rob", surname="Maklowicz"},
            };
            using (var db = new RaportContext())
            {
                users.ForEach(u => db.User.Add(u));
                db.SaveChanges();
            }
            var projects = new List<Activity>{
                new Activity{activityId=1, managerId=1, code="yang12", budget=1200, active=true, name="Super Yang"},
                new Activity{activityId=2, managerId=1, code="yang13", budget=1234, active=false, name="Super Yang 2"},
                new Activity{activityId=3, managerId=3, code="cyber", budget=131233, active=true, name="Cyber bug 3"},
                new Activity{activityId=4, managerId=3, code="cerber", budget=1234, active=true, name="Hades 22"},
                new Activity{activityId=5, managerId=3, code="bang", budget=51234, active=true, name="Gang plank"},
                new Activity{activityId=6, managerId=2, code="cang", budget=41234, active=true, name="Test test"},
                new Activity{activityId=7, managerId=5, code="dang", budget=31234, active=true, name="Hobbit"},
            };
            using (var db = new RaportContext())
            {
                projects.ForEach(p => db.Project.Add(p));
                db.SaveChanges();
            }
            // TODO zaladowac do bazy danych

            // var raports = new List<Raport>{
            //     new Raport{raportId=1, userId=3, frozen=}
            // };

        }

    }

}