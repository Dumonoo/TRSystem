using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TimeReportingSystem.Models
{
    public class Entry
    {
        public int entryId { get; set; }
        [Required]
        public string date { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]+", ErrorMessage ="Proszę podać prawidłową liczbę!")]
        public int time { get; set; }
        [StringLength(2000)]
        public string description { get; set; }

        public int  activityId { get; set; }
        public Activity Activity { get; set; }
        public int? subactivityId { get; set; }
        public Subactivity Subactivity { get; set; }
        public int raportId { get; set; }
        public Raport Raport { get; set; }
        [Timestamp]
        public DateTime TimeStamp {get; set;}
        
    }
}