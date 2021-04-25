using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnitedSales.Areas.Admin.Models
{
    public abstract class SchemaLog
    {
        [Column(TypeName="nvarchar(100)")]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [Column(TypeName="nvarchar(100)")]
        public string UpdatedBy { get; set; }
        
        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public string ReturnUrl { get; set; }

    }
}