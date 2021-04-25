using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace UnitedSales.Areas.Admin.Models
{
    public class Project : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Project Name")]
        [Required(ErrorMessage = "Please enter Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Channel Head")]
        public string ChannelHead { get; set; }

        [Display(Name = "Per Square yard")]
        public int ChannelHeadPerSqYard { get; set; }

        [Display(Name = "Per Square feet")]
        public int ChannelHeadPerSqft { get; set; }

        [Display(Name = "Business Ambassador")]
        public string Ambassador { get; set; }

        [Display(Name = "Per Square yard")]
        public int AmbassadorPerSqYard { get; set; }

        [Display(Name = "Per Square feet")]
        public int AmbassadorPerSqft { get; set; }

        [Display(Name = "CH - BA")]
        public string CHBA { get; set; }

        [Display(Name = "Per Square yard")]
        public int CHBAPerSqYard { get; set; }

        [Display(Name = "Per Square feet")]
        public int CHBAPerSqft { get; set; }

        public string Telecaller { get; set; }

        [Display(Name = "Fixed")]
        public int Fixed { get; set; }
    }
}