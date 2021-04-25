using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UnitedSales.Areas.Admin.Models;
using UnitedSales.Models;
using Microsoft.AspNetCore.Authorization;

namespace UnitedSales.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CallingController : Controller
    {
        private readonly ILogger<CallingController> _logger;
        

        public CallingController(ILogger<CallingController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
