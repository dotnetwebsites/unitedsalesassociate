using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace UnitedSales.Areas.Admin.Models
{
    public class LeadsAllocation : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select Lead")]
        [Display(Name = "Lead")]
        public string LeadId { get; set; }

        [Required(ErrorMessage = "Please select Caller")]
        [Display(Name = "Telecaller")]
        public string CallerId { get; set; }
    }
}