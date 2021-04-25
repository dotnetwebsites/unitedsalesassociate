using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace UnitedSales.Areas.Admin.Models
{
    public class ProjectArea : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        [Display(Name = "Square yard")]
        [Required(ErrorMessage = "Please enter square yard")]
        public int? SqYard { get; set; }

        [Display(Name = "Square feet")]
        [Required(ErrorMessage = "Please enter square feet")]
        public int? Sqft { get; set; }
    }
}