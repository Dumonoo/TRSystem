using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TimeReportingSystem.Models
{
    public class Subactivity
    {   
        public int subactivityId { get; set; }
        
        [Required (ErrorMessage = "Należy podać kod podaktywnośći")]
        [RegularExpression("[a-zA-Z0-9-_]+", ErrorMessage = "Nazwa podaktywności nie może zawierać spacjii!")]
        [Remote("SubCodeUniqueness", "Projects", AdditionalFields ="projectCode")]
        public string code { get; set; }

        public int  activityId { get; set; }
        public virtual Activity Activity { get; set; }
        [Timestamp]
        public DateTime TimeStamp {get; set;}
        
    }
}