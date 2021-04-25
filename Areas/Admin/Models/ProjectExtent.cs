using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace UnitedSales.Areas.Admin.Models
{
    public class ProjectExtent : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter project")]
        public int ProjectId { get; set; }

        // [Display(Name = "Total Property in Sqr Yrd")]
        [NotMapped]
        public int TotSqYard { get; set; }

        [NotMapped]
        public int TotSqft { get; set; }

        public string YardSft { get; set; }

        [Display(Name = "Total Property")]
        public double TotValue { get; set; }

        [NotMapped]
        public string PEID { get; set; }

        [NotMapped]
        public string PEVAL { get; set; }

        [NotMapped]
        public string ExtentValue
        {
            get
            {
                return TotValue + (YardSft == "Y" ? " Square Yard" : " Square Feet");
            }
        }
    }
}
