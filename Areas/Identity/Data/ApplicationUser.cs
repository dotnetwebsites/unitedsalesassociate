using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using UnitedSales.Utilities;

namespace UnitedSales.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Select User")]
        public string EmployeeType { get; set; }

        [Display(Name = "Channel Head")]
        public string ChannelHeadId { get; set; }
        
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [PersonalData]
        [Display(Name = "Father Name")]
        [Column(TypeName = "nvarchar(100)")]
        public string FatherName { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [PersonalData]
        public string Gender { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(500)")]
        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string Address { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Designation { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Joining")]
        public DateTime DateofJoining { get; set; }

        public string PAN { get; set; }

        [Display(Name = "Aadhar No")]
        [Column(TypeName = "nvarchar(20)")]
        [MaxLength(12, ErrorMessage = "Max 16 digit")]
        [MinLength(12, ErrorMessage = "Please enter 12 digit")]
        public string AadhaarNo { get; set; }

        [Display(Name = "Highest Qualification")]
        [Column(TypeName = "nvarchar(200)")]
        public string HighestQualification { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                if (MiddleName != null)
                    return FirstName + " " + MiddleName + " " + LastName;
                else
                    return FirstName + " " + LastName;
            }
        }

        [NotMapped]
        public string EmpCodeName
        {
            get
            {
                return EmployeeCode + " - " + FullName;
            }
        }

        [NotMapped]
        [Display(Name = "Profile Image")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile Avatar { get; set; }
        public bool IsActive { get; set; }
    
    }
}
