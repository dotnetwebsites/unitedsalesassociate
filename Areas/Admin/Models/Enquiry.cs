using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UnitedSales.Utilities;
using Microsoft.AspNetCore.Http;

namespace UnitedSales.Areas.Admin.Models
{
    public class Enquiry : SchemaLog
    {
        [Key]
        public int Id { get; set; }
               
        [Column(TypeName = "nvarchar(100)")]
        public string EmpCode { get; set; }

        public string LeadAllocTo { get; set; }

        //[Required(ErrorMessage = "Required first name")]
        [Display(Name = "First Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Required last name")]
        [Display(Name = "Last Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Required email address")]
        [Display(Name = "Email Address")]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Required(ErrorMessage = "Required mobile no")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "Mobile no must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
        [Display(Name = "Mobile No")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Zone")]
        [Column(TypeName = "nvarchar(200)")]
        public string SpouseName { get; set; }

        [Display(Name = "City")]
        //[Required(ErrorMessage = "Please select city.")]
        public int? CityId { get; set; }

        [Display(Name = "State")]
        //[Required(ErrorMessage = "Please select state.")]
        public int? StateId { get; set; }

        [Display(Name = "Alternate No / Whatsapp No")]
        [Column(TypeName = "nvarchar(20)")]
        //[Required(ErrorMessage = "Required Alternate No / Whatsapp No")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "No must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Alternate No / Whatsapp No must be numeric")]
        public string AlternateNo { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    public class EnquiryViewModel
    {
     
        [Required(ErrorMessage = "Please choose excel file")]
        [Display(Name = "Excel File")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".xlsx", ".xls" })]
        public IFormFile ExcelFile { get; set; }

        [NotMapped]
        public bool IsOnlyMobNo { get; set; }
    }
}