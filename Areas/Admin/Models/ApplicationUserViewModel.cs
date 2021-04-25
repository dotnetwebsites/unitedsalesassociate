using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using UnitedSales.Areas.Identity.Data;

namespace UnitedSales.Models
{
    public class ApplicationUserViewModel
    {
        public ApplicationUser User { get; set; }                
    }
}