using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace UnitedSales.Areas.Admin.Models
{
    public class Location : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Location Name")]
        [Column(TypeName = "nvarchar(500)")]
        [Required(ErrorMessage = "Please enter Location Name")]
        public string LocationName { get; set; }
    }
}