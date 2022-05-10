using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeReportingSystem.Models
{
    public class Activity
    {
        public int activityId { get; set; }


        [Required(ErrorMessage = "Proszę podać kod projektu")]
        [RegularExpression("[a-zA-Z0-9-_]+", ErrorMessage = "Nazwa projektu może składać sie z dużych i małych liter, cyfr - i _")]
        [Remote("ProjCodeUniqueness", "Projects")]
        [StringLength(30)]
        public string code { get; set; }

        public int managerId { get; set; }
        [ForeignKey("managerId")]
        public User manager { get; set; }

        public virtual ICollection<Subactivity> subactivities { get; set; }
        public virtual ICollection<Entry> entries { get; set; }
        public virtual ICollection<Accepted> accepted { get; set; }

        [Required(ErrorMessage = "Proszę podać nazwę projektu")]
        [StringLength(90)]
        public string name { get; set; }
        [Required(ErrorMessage = "Proszę podać budżet projektu")]
        [RegularExpression("[0-9]+\\.?[0-9]?", ErrorMessage = "Budżet może miec jedno miejsce po przecinku")]
        [Range(0, 8000000)]

        public int budget { get; set; }
        public bool active { get; set; }
        [Timestamp]
        public DateTime TimeStamp {get; set;}

    }
}
