using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace UnitedSales.Areas.Admin.Models
{
    public class SendSMS : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        //[Required(ErrorMessage = "Please enter message to view")]
        public string ViewMsg { get; set; }
        
        [Column(TypeName = "nvarchar(1000)")]
        [Required(ErrorMessage = "Please enter message")]
        public string Message { get; set; }

        public bool IsActive { get; set; }
    }

    public class SendSMSView : SendSMS
    {
        public string ProjectName { get; set; }
    }
}