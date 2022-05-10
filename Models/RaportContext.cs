using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TimeReportingSystem.Models;
using System.Configuration;

namespace TimeReportingSystem.Models{

    public class RaportContext : DbContext
    {

        public DbSet<User> User {get; set;}
        public DbSet<Raport> Raport { get; set; }
        public DbSet<Accepted> AcceptedTime { get; set; }
        public DbSet<Activity> Project { get; set; }
        public DbSet<Entry> RaportEntry { get; set; }
        public DbSet<Subactivity> Subactivity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;port=3306;database=TRSystem;username=NTR21;password=petclinic");
        }

    }
}
