using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace TimeReportingSystem.Models
{
    public class Raport
    {
        public int raportId { get; set; }
        public bool frozen { get; set; }
        public int year { get; set; }
        public int month { get; set; }

        public int userId { get; set; }

        public virtual User user { get; set; }
        public virtual ICollection<Entry> entries { get; set; }
        public virtual ICollection<Accepted> accepted { get; set; }


    }
}