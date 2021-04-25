using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UnitedSales.Areas.Identity.Data;
using UnitedSales.Areas.Admin.Models;

namespace UnitedSales.Areas.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Enquiry>()
                .HasIndex(u => new { u.PhoneNumber })
                .IsUnique();
        }

        public DbSet<MailLibrary> MailLibraries { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        //public DbSet<DocumentList> DocumentLists { get; set; }
        //public DbSet<MyDocument> MyDocuments { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectExtent> ProjectExtents { get; set; }
        public DbSet<ProjectArea> ProjectAreas { get; set; }
        public DbSet<Location> Locations { get; set; }

        // public DbSet<NoticeBoard> NoticeBoard { get; set; }
        // public DbSet<Brochure> Brochures { get; set; }

        public DbSet<LeadsAllocation> LeadsAllocations { get; set; }
        public DbSet<TeleCalling> TeleCallings { get; set; }

        public DbSet<SendSMS> SendSMS { get; set; }
    }
}
