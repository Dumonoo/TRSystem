using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TimeReportingSystem.Models
{
    public class Accepted
    {
        public int acceptedId {get; set;}
        
        [Required]
        public int time { get; set; }
        
        public int raportId { get; set; }
        public virtual Raport Raport { get; set; }
        public int activityId { get; set; }
        public virtual Activity Activity { get; set; }
        [Timestamp]
        public DateTime TimeStamp {get; set;}

    }   
    
}