using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace UnitedSales.Areas.Admin.Models
{
    public class TeleCalling : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Enquiry")]
        public int EnquiryId { get; set; }

        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Extent")]
        public int ExtentId { get; set; }

        public string Status { get; set; }

        [Display(Name = "Follow up date")]
        public DateTime? FollowUpDate { get; set; }
        public string CallerId { get; set; }

        public string Remark { get; set; }
    }

    public class TeleCallingView : SchemaLog
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Alternate No / Whatsapp No")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "No must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Alternate No / Whatsapp No must be numeric")]
        public string AlternateNo { get; set; }

        [Display(Name = "City")]
        public int? CityId { get; set; }

        [Display(Name = "State")]
        public int? StateId { get; set; }

        [Display(Name = "Zone")]
        public string SpouseName { get; set; }

        [Display(Name = "Enquiry")]
        public int EnquiryId { get; set; }

        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Extent")]
        public int ExtentId { get; set; }

        public string Status { get; set; }

        [Display(Name = "Follow up date")]
        public DateTime? FollowUpDate { get; set; }
        public string CallerId { get; set; }

        public string Remark { get; set; }
        
        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    public class WalletSummary
    {
        public int Id { get; set; }
        public string EmpCodeName { get; set; }
        public int ClosedCount { get; set; }
        public double TotalIncentives { get; set; }
    }
}