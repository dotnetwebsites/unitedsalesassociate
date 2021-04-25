using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitedSales.Areas.Identity.Data;

[assembly: HostingStartup(typeof(UnitedSales.Areas.Identity.IdentityHostingStartup))]
namespace UnitedSales.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection")));

                // services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //     .AddEntityFrameworkStores<ApplicationDbContext>();
                services.AddDefaultIdentity<ApplicationUser>(
                    options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireLowercase = true;
                        options.Password.RequireUppercase = true;
                        options.Password.RequireDigit = true;
                        options.Password.RequiredLength = 8;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
            });
        }
    }
}