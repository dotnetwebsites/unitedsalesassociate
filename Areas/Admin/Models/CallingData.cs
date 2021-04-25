using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnitedSales.Areas.Admin.Models
{
    public class CallingData:SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Allocation Date")]
        public DateTime AllocDate { get; set; }

        [Display(Name="Allocation Name")]
        [Column(TypeName="nvarchar(512)")]
        public string AllocTo { get; set; }

        [Display(Name="Customer Name")]
        [Column(TypeName="nvarchar(200)")]
        public string CustomerName { get; set; }

        [Display(Name="Mobile No.")]
        [Column(TypeName="nvarchar(20)")]
        public string PhoneNumber { get; set; }

        [Display(Name="Alternate No.")]
        [Column(TypeName="nvarchar(20)")]
        public string AlternateNumber { get; set; }

        [Display(Name="Status")]
        [Column(TypeName="nvarchar(100)")]
        public string Status { get; set; }

        [Display(Name="Customer Feedback")]
        [Column(TypeName="nvarchar(500)")]
        public string Remark { get; set; }

        [Display(Name="Follow Up Date")]
        public DateTime FollowUpDate { get; set; }       
    }
    
}