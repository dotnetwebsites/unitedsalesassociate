using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UnitedSales.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UnitedSales.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UnitedSales.Services;
using ExcelDataReader;
using System.Net;
using AutoMapper.Configuration;
using AutoMapper;

namespace UnitedSales.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DiallingController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailService _emailSender;
        private static int count = 0;
        private static bool Inserted = false;
        public Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IMapper _mapper;
        
        public DiallingController(ILogger<HomeController> logger,
                                ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                UserManager<ApplicationUser> userManager,
                                IMailService emailSender,
                                Microsoft.Extensions.Configuration.IConfiguration config,
                                IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = config;
            _mapper = mapper;
        }

        [Authorize(Roles = "admin,Telecaller,CHBA,Ambassador,ChannelHead")]
        public IActionResult Index()
        {
            //ViewBag.ProjectExt = new SelectList(_repository.ProjectExtents, "Id", "ExtentValue");
            //ViewBag.ProjectName = new SelectList(await _repository.Projects.ToListAsync(), "Id", "ProjectName");
            return View();
        }

        [Authorize(Roles = "admin,Telecaller,CHBA,Ambassador,ChannelHead,CRM,Account")]
        public IActionResult ListOfClosures()
        {
            return View();
        }

        [Authorize(Roles = "admin,Telecaller,CHBA,Ambassador,ChannelHead,CRM,Account")]
        public IActionResult ListOfFollowUps()
        {
            return View();
        }

        [Authorize(Roles = "admin,Telecaller,CHBA,Ambassador,ChannelHead,CRM,Account")]
        public IActionResult ListOfNotInterested()
        {
            return View();
        }

        [Authorize(Roles = "admin,Telecaller,CHBA,Ambassador,ChannelHead,CRM,Account")]
        //[Authorize(Roles = "admin,CRM")]
        public IActionResult ListOfSiteVisit()
        {
            return View();
        }

        [Authorize(Roles = "admin,Telecaller,CHBA,Ambassador,ChannelHead,CRM,Account")]
        //[Authorize(Roles = "admin,Account")]
        public IActionResult ListOfAccounts()
        {
            return View();
        }

        public async Task<IActionResult> Index1()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "admin");

            if (isAdmin)
            {
                var Enquiries = await _repository.Enquiries.ToListAsync();
                return View(Enquiries);
            }
            else
            {
                if (await _repository.LeadsAllocations.AnyAsync(p => p.CallerId == user.Id))
                {
                    var caller = _repository.LeadsAllocations.FirstOrDefault(p => p.CallerId == user.Id);
                    var lead = await _userManager.FindByIdAsync(caller.LeadId);
                    var Enquiries = await _repository.Enquiries.Where(p => p.EmpCode == lead.EmployeeCode).ToListAsync();
                    return View(Enquiries);
                }
                else
                    return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin,Telecaller,CHBA,Ambassador,ChannelHead")]
        public async Task<IActionResult> Wallet(string lid = null)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.IsAdmin = await _userManager.IsInRoleAsync(user, "admin");
            double incentives = 0;
            if (await _userManager.IsInRoleAsync(user, "telecaller"))
            {
                var teleCallings = _repository.TeleCallings.Where(p => p.CallerId == user.Id && p.Status == "CLOSED");

                foreach (var item in teleCallings)
                {
                    var proj = await _repository.Projects.FindAsync(item.ProjectId);

                    incentives += proj.Fixed;
                }

                ViewBag.Wallet = incentives;
            }
            else if (await _userManager.IsInRoleAsync(user, "ambassador") ||
                     await _userManager.IsInRoleAsync(user, "CHBA")
                     //|| await _userManager.IsInRoleAsync(user, "ChannelHead")
                     )
            {
                var teleCallings = from c in _repository.TeleCallings
                                   join e in _repository.Enquiries on c.EnquiryId equals e.Id
                                   where e.EmpCode == user.EmployeeCode && c.Status == "CLOSED"
                                   select new { Id = c.Id, ProjectId = c.ProjectId, ExtentId = c.ExtentId };

                foreach (var item in teleCallings)
                {
                    var proj = await _repository.Projects.FindAsync(item.ProjectId);
                    var project = await _repository.ProjectExtents.FindAsync(item.ExtentId);

                    if (project.YardSft == "Y")
                    {
                        if (await _userManager.IsInRoleAsync(user, "ambassador"))
                            incentives += (project.TotValue * proj.AmbassadorPerSqYard);
                        else if (await _userManager.IsInRoleAsync(user, "CHBA"))
                            incentives += (project.TotValue * proj.CHBAPerSqYard);
                        // else if (await _userManager.IsInRoleAsync(user, "ChannelHead"))
                        //     incentives += (project.TotValue * proj.ChannelHeadPerSqYard);
                    }
                    else if (project.YardSft == "F")
                    {
                        if (await _userManager.IsInRoleAsync(user, "ambassador"))
                            incentives += (project.TotValue * proj.AmbassadorPerSqft);
                        else if (await _userManager.IsInRoleAsync(user, "CHBA"))
                            incentives += (project.TotValue * proj.CHBAPerSqft);
                        // else if (await _userManager.IsInRoleAsync(user, "ChannelHead"))
                        //     incentives += (project.TotValue * proj.ChannelHeadPerSqft);
                    }
                }

                ViewBag.Wallet = incentives;
            }
            else if (await _userManager.IsInRoleAsync(user, "ChannelHead"))
            {
                var teleCallings = from c in _repository.TeleCallings
                                   join e in _repository.Enquiries on c.EnquiryId equals e.Id
                                   where e.EmpCode == user.EmployeeCode && c.Status == "CLOSED"
                                   select new { Id = c.Id, ProjectId = c.ProjectId, ExtentId = c.ExtentId };

                foreach (var item in teleCallings)
                {
                    var proj = await _repository.Projects.FindAsync(item.ProjectId);
                    var project = await _repository.ProjectExtents.FindAsync(item.ExtentId);

                    if (project.YardSft == "Y")
                    {
                        if (await _userManager.IsInRoleAsync(user, "ChannelHead"))
                            incentives += (project.TotValue * proj.ChannelHeadPerSqYard);
                    }
                    else if (project.YardSft == "F")
                    {
                        if (await _userManager.IsInRoleAsync(user, "ChannelHead"))
                            incentives += (project.TotValue * proj.ChannelHeadPerSqft);
                    }
                }

                var chUser = await _repository.Users.FirstOrDefaultAsync(p => p.ChannelHeadId == user.Id);

                if (chUser != null)
                {
                    var baCalls = from c in _repository.TeleCallings
                                  join e in _repository.Enquiries on c.EnquiryId equals e.Id
                                  where e.EmpCode == chUser.EmployeeCode && c.Status == "CLOSED"
                                  select new { Id = c.Id, ProjectId = c.ProjectId, ExtentId = c.ExtentId };

                    if (baCalls != null)
                    {
                        foreach (var item in baCalls)
                        {
                            var proj = await _repository.Projects.FindAsync(item.ProjectId);
                            var project = await _repository.ProjectExtents.FindAsync(item.ExtentId);

                            if (project.YardSft == "Y")
                            {
                                if (await _userManager.IsInRoleAsync(user, "ChannelHead"))
                                    incentives += (project.TotValue * proj.ChannelHeadPerSqYard);
                            }
                            else if (project.YardSft == "F")
                            {
                                if (await _userManager.IsInRoleAsync(user, "ChannelHead"))
                                    incentives += (project.TotValue * proj.ChannelHeadPerSqft);
                            }
                        }
                    }
                }

                ViewBag.Wallet = incentives;
            }
            else
            {
                ViewBag.Wallet = incentives;
            }

            if (ViewBag.IsAdmin)
            {
                double inc = 0; int count = 0;
                
                List<ApplicationUser> users = new List<ApplicationUser>();

                foreach (var u in _userManager.Users)
                {
                    if (!(await _userManager.IsInRoleAsync(u, "crm") ||
                            await _userManager.IsInRoleAsync(u, "account")))
                        users.Add(u);
                }

                ViewBag.LeadId = new SelectList(users, "Id", "EmpCodeName");
                if (lid != null)
                {
                    var u = await _repository.Users.FindAsync(lid);

                    ///////////////////////////////////////////////////////////////

                    if (await _userManager.IsInRoleAsync(u, "telecaller"))
                    {
                        var teleCallings = _repository.TeleCallings.Where(p => p.CallerId == u.Id && p.Status == "CLOSED");

                        foreach (var item in teleCallings)
                        {
                            var proj = await _repository.Projects.FindAsync(item.ProjectId);

                            inc += proj.Fixed;
                        }

                        //inc = incentives;
                        count = teleCallings.Count();
                    }
                    else if (await _userManager.IsInRoleAsync(u, "ambassador") ||
                            await _userManager.IsInRoleAsync(u, "CHBA"))
                    {
                        var teleCallings = from c in _repository.TeleCallings
                                           join e in _repository.Enquiries on c.EnquiryId equals e.Id
                                           where e.EmpCode == u.EmployeeCode && c.Status == "CLOSED"
                                           select new { Id = c.Id, ProjectId = c.ProjectId, ExtentId = c.ExtentId };

                        foreach (var item in teleCallings)
                        {
                            var proj = await _repository.Projects.FindAsync(item.ProjectId);
                            var project = await _repository.ProjectExtents.FindAsync(item.ExtentId);

                            if (project.YardSft == "Y")
                            {
                                if (await _userManager.IsInRoleAsync(u, "ambassador"))
                                    inc += (project.TotValue * proj.AmbassadorPerSqYard);
                                else if (await _userManager.IsInRoleAsync(u, "CHBA"))
                                    inc += (project.TotValue * proj.CHBAPerSqYard);
                                else if (await _userManager.IsInRoleAsync(u, "ChannelHead"))
                                    inc += (project.TotValue * proj.ChannelHeadPerSqYard);
                            }
                            else if (project.YardSft == "F")
                            {
                                if (await _userManager.IsInRoleAsync(u, "ambassador"))
                                    inc += (project.TotValue * proj.AmbassadorPerSqft);
                                else if (await _userManager.IsInRoleAsync(u, "CHBA"))
                                    inc += (project.TotValue * proj.CHBAPerSqft);
                                else if (await _userManager.IsInRoleAsync(u, "ChannelHead"))
                                    inc += (project.TotValue * proj.ChannelHeadPerSqft);
                            }
                        }

                        //inc = incentives;
                        count = teleCallings.Count();
                    }
                    else if (await _userManager.IsInRoleAsync(u, "ChannelHead"))
                    {
                        var teleCallings = from c in _repository.TeleCallings
                                           join e in _repository.Enquiries on c.EnquiryId equals e.Id
                                           where e.EmpCode == u.EmployeeCode && c.Status == "CLOSED"
                                           select new { Id = c.Id, ProjectId = c.ProjectId, ExtentId = c.ExtentId };

                        foreach (var item in teleCallings)
                        {
                            var proj = await _repository.Projects.FindAsync(item.ProjectId);
                            var project = await _repository.ProjectExtents.FindAsync(item.ExtentId);

                            if (project.YardSft == "Y")
                            {
                                if (await _userManager.IsInRoleAsync(u, "ChannelHead"))
                                    inc += (project.TotValue * proj.ChannelHeadPerSqYard);
                            }
                            else if (project.YardSft == "F")
                            {
                                if (await _userManager.IsInRoleAsync(u, "ChannelHead"))
                                    inc += (project.TotValue * proj.ChannelHeadPerSqft);
                            }
                        }

                        var chUser = await _repository.Users.FirstOrDefaultAsync(p => p.ChannelHeadId == u.Id);

                        if (chUser != null)
                        {
                            var baCalls = from c in _repository.TeleCallings
                                          join e in _repository.Enquiries on c.EnquiryId equals e.Id
                                          where e.EmpCode == chUser.EmployeeCode && c.Status == "CLOSED"
                                          select new { Id = c.Id, ProjectId = c.ProjectId, ExtentId = c.ExtentId };

                            foreach (var item in baCalls)
                            {
                                var proj = await _repository.Projects.FindAsync(item.ProjectId);
                                var project = await _repository.ProjectExtents.FindAsync(item.ExtentId);

                                if (project.YardSft == "Y")
                                {
                                    if (await _userManager.IsInRoleAsync(u, "ChannelHead"))
                                        inc += (project.TotValue * proj.ChannelHeadPerSqYard);
                                }
                                else if (project.YardSft == "F")
                                {
                                    if (await _userManager.IsInRoleAsync(u, "ChannelHead"))
                                        inc += (project.TotValue * proj.ChannelHeadPerSqft);
                                }
                            }

                            count = baCalls.Count();
                        }

                        count += teleCallings.Count();
                        //inc = incentives;
                        
                    }
                    else
                    {
                        inc = incentives;
                    }

                    WalletSummary ws = new WalletSummary();
                    ws.EmpCodeName = u.EmpCodeName;
                    ws.ClosedCount = count;
                    ws.TotalIncentives = inc;

                    ViewData["Summary"] = ws;

                }
                ///////////////////////////////////////////////////////////////                
            }

            return View();
        }


        [HttpGet]
        [Authorize(Roles = "admin,Telecaller")]
        public IActionResult Create()
        {
            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName");
            return View();
        }

        [HttpGet]
        public JsonResult LoadCities(int id)
        {
            var cities = _repository.Cities.Where(p => p.StateId == id).ToList();
            return Json(new SelectList(cities, "Id", "CityName"));
        }

        [HttpPost]
        [Authorize(Roles = "admin,Telecaller")]
        public async Task<IActionResult> Create(Enquiry enquiry)
        {
            if (ModelState.IsValid)
            {
                // if (enquiry.CityId <= 0 || enquiry?.CityId == null)
                // {
                //     ModelState.AddModelError(string.Empty, "Please select city and try again.");
                //     ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", enquiry.StateId);
                //     ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", enquiry.CityId);
                //     return View(enquiry);
                // }

                var user = await _userManager.GetUserAsync(User);

                enquiry.CreatedBy = user.UserName;
                //enquiry.Username = user.UserName;
                enquiry.CreatedDate = DateTime.Now;
                enquiry.EmpCode = user.EmployeeCode;

                _repository.Enquiries.Add(enquiry);
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", enquiry.StateId);
            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", enquiry.CityId);
            return View(enquiry);
        }


        [HttpGet]
        public async Task<IActionResult> UpdateSiteVisit(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }

            TeleCalling teleCalling = await _repository.TeleCallings.FindAsync(id);
            return View(teleCalling);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSiteVisit(TeleCalling teleCalling)
        {
            if (ModelState.IsValid)
            {
                TeleCalling call = await _repository.TeleCallings.FindAsync(teleCalling.Id);
                
                call.Status = teleCalling.Status;

                await _repository.SaveChangesAsync();
                return RedirectToAction("ListOfSiteVisit");
            }

            return View(teleCalling);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAccount(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }

            TeleCalling teleCalling = await _repository.TeleCallings.FindAsync(id);
            return View(teleCalling);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccount(TeleCalling teleCalling)
        {
            if (ModelState.IsValid)
            {
                TeleCalling call = await _repository.TeleCallings.FindAsync(teleCalling.Id);
                
                call.Status = teleCalling.Status;

                await _repository.SaveChangesAsync();
                return RedirectToAction("ListOfAccounts");
            }

            return View(teleCalling);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }
            Enquiry enquiry = await _repository.Enquiries.FindAsync(id);
            return View(enquiry);
        }

        [HttpGet]
        public JsonResult LoadExtent(int id)
        {
            var projectextents = _repository.ProjectExtents.Where(p => p.ProjectId == id).ToList();
            //ViewBag.ExtentId = new SelectList(_repository.ProjectExtents, "Id", "ExtentValue");
            return Json(new SelectList(projectextents, "Id", "ExtentValue"));
        }

        [HttpGet]
        //[Authorize]
        [Authorize(Roles = "admin,Telecaller")]
        public async Task<IActionResult> SendMessage(string mob = null, int? msg = null)
        {
            if(mob ==null)
            {
                return Json("Mobile no not valid.");
            }

            if(msg == null)
            {
                return Json("Please select message.");
            }

            var message = await _repository.SendSMS.FindAsync(msg);

            if(message == null)
            {
                return Json("Message not exists.");
            }

            var sURL = _configuration["SmsUrl"].ToString() +
                "username=" + _configuration["SMSUser"].ToString() +
                "&message=" + message.Message +
                "&sendername=" + _configuration["SenderName"].ToString() +
                "&smstype=" + _configuration["SmsType"].ToString() +
                "&numbers=" + mob +
                "&apikey=" + _configuration["ApiKey"].ToString();

            try
            {
                using (WebClient client = new WebClient())
                {

                    string s = client.DownloadString(sURL);

                    var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(s);
                    int n = responseObject.Status;                    
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error in sending Message");
                ex.ToString();
            }

            return Json("SMS has been sent");
        }

        [HttpGet]
        //[Authorize]
        [Authorize(Roles = "admin,Telecaller")]
        public async Task<IActionResult> Edit(int? enqid, int? id = null)
        {
            // if (id == null)
            // {
            //     ViewBag.LocationId = new SelectList(_repository.Locations, "Id", "LocationName");
            //     ViewBag.ProjectId = new SelectList(_repository.Projects, "Id", "ProjectName");
            //     ViewBag.Messages = await _repository.SendSMS.Where(p => p.IsActive).ToListAsync();

            //     ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName");
            //     ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName");
            //     return View();
            // }

            var teleCalling = _mapper.Map<TeleCallingView>(await _repository.Enquiries.FindAsync(enqid));

            var cal = await _repository.TeleCallings.FindAsync(id);
            
            if(cal != null)
            {
                teleCalling.LocationId = cal.LocationId;
                teleCalling.ProjectId = cal.ProjectId;
                teleCalling.ExtentId = cal.ExtentId;
                teleCalling.Status = cal.Status;
                teleCalling.FollowUpDate = cal.FollowUpDate;
                teleCalling.Remark = cal.Remark;
            }
            
            ViewBag.LocationId = new SelectList(_repository.Locations, "Id", "LocationName", teleCalling.LocationId);
            ViewBag.ProjectId = new SelectList(_repository.Projects, "Id", "ProjectName", teleCalling.ProjectId);
            ViewBag.ExtentId = new SelectList(_repository.ProjectExtents.Where(p => p.ProjectId == teleCalling.ProjectId), "Id", "ExtentValue", teleCalling.ExtentId);
            
            ViewBag.Messages = await _repository.SendSMS.Where(p => p.IsActive).ToListAsync();

            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", teleCalling.StateId);
            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", teleCalling.CityId);

            return View(teleCalling);
        }

        [HttpPost]
        //[Authorize]
        [Authorize(Roles = "admin,Telecaller")]
        public async Task<IActionResult> Edit(int enqid, TeleCallingView teleCalling)
        {
            
            if (ModelState.IsValid)
            {
                var model = await _repository.Enquiries.FindAsync(enqid);
                model.FirstName = teleCalling.FirstName;
                model.LastName = teleCalling.LastName;
                model.Email = teleCalling.Email;
                model.AlternateNo = teleCalling.AlternateNo;
                model.CityId = teleCalling.CityId;
                model.StateId = teleCalling.StateId;
                model.SpouseName = teleCalling.SpouseName;
                //await _repository.SaveChangesAsync();

                if (await _repository.TeleCallings.AnyAsync(p => p.EnquiryId == enqid))
                {

                    var tc = await _repository.TeleCallings.FirstOrDefaultAsync(p => p.EnquiryId == enqid);
                    tc.UpdatedBy = User.Identity.Name;
                    tc.UpdatedDate = DateTime.Now;

                    tc.LocationId = teleCalling.LocationId;
                    tc.ProjectId = teleCalling.ProjectId;
                    tc.ExtentId = teleCalling.ExtentId;
                    tc.Status = teleCalling.Status;
                    tc.FollowUpDate = teleCalling.FollowUpDate;
                    tc.Remark = teleCalling.Remark;
                    //await _repository.SaveChangesAsync();
                }
                else
                {                    
                    TeleCalling tc = new TeleCalling();
                    tc.EnquiryId = enqid;
                    tc.LocationId = teleCalling.LocationId;
                    tc.ProjectId = teleCalling.ProjectId;
                    tc.ExtentId = teleCalling.ExtentId;
                    tc.Status = teleCalling.Status;
                    tc.CreatedBy = User.Identity.Name;
                    tc.CreatedDate = DateTime.Now;
                    tc.CallerId = _userManager.GetUserId(User);
                    tc.Remark = teleCalling.Remark;
                    tc.FollowUpDate = teleCalling.FollowUpDate;
                    
                    _repository.TeleCallings.Add(tc);                    
                }

                await _repository.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.LocationId = new SelectList(_repository.Locations, "Id", "LocationName", teleCalling.LocationId);
            ViewBag.ProjectId = new SelectList(_repository.Projects, "Id", "ProjectName", teleCalling.ProjectId);
            ViewBag.ExtentId = new SelectList(_repository.ProjectExtents, "Id", "ExtentValue", teleCalling.ExtentId);

            ViewBag.Messages = await _repository.SendSMS.Where(p => p.IsActive).ToListAsync();

            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", teleCalling.StateId);
            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", teleCalling.CityId);

            return View(teleCalling);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int? projectId)
        {
            if(projectId == null)
                return Json("--Select--");

            return Json(await _repository.SendSMS.Where(p => p.IsActive && p.ProjectId == projectId).ToListAsync());
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult BulkUpload()
        {
            if (Inserted)
            {
                if (count <= 0)
                    ViewBag.RecordSaved = "No records has been inserted";
                else
                    ViewBag.RecordSaved = count + " Records successfully inserted";

                count = 0;
                Inserted = false;
            }

            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> BulkUpload(EnquiryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // if (model.Month == "" || model.Month == null)
                // {
                //     ModelState.AddModelError(string.Empty, "Please select month and try again.");
                //     return View(model);
                // }

                // if (model.Year < 2019)
                // {
                //     ModelState.AddModelError(string.Empty, "Please select year and try again.");
                //     return View(model);
                // }

                string directory = Path.Combine(_webHostEnvironment.WebRootPath, "files");

                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                string fileName = directory + "\\" + model.ExcelFile.FileName;

                using (FileStream fileStream = System.IO.File.Create(fileName))
                {
                    model.ExcelFile.CopyTo(fileStream);
                    fileStream.Flush();
                }

                int cnt = 0;

                var enquiries = await this.GetRecAsync(model);
                foreach (var enquiry in enquiries)
                {
                    var exists = _repository.Enquiries.Any(p =>
                     p.PhoneNumber == enquiry.PhoneNumber ||
                     p.Email == enquiry.Email);

                    if (!exists)
                    {
                        _repository.Enquiries.Add(enquiry);
                        await _repository.SaveChangesAsync();
                        cnt++;
                    }
                }

                Inserted = true;
                count = cnt;
                return RedirectToAction(nameof(BulkUpload));
            }

            return View(model);
        }

        public async Task<List<Enquiry>> GetRecAsync(EnquiryViewModel model)
        {
            List<Enquiry> enquiries = new List<Enquiry>();

            var user = await _userManager.GetUserAsync(User);

            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + model.ExcelFile.FileName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        if (!reader.GetValue(0).ToString().Contains("SNO"))
                        {
                            bool exists = _repository.Enquiries.Any(p =>
                            p.PhoneNumber == reader.GetValue(4).ToString() ||
                            p.Email == reader.GetValue(4).ToString());

                            if (exists)
                                continue;

                            if (!_repository.Cities.Any(p => p.CityName == reader.GetValue(16).ToString()))
                                continue;

                            var city = _repository.Cities.First(p => p.CityName == reader.GetValue(16).ToString());
                            var enquiry = new Enquiry()
                            {
                                FirstName = reader.GetValue(1).ToString(),
                                LastName = reader.GetValue(2).ToString(),
                                Email = reader.GetValue(3).ToString(),
                                PhoneNumber = reader.GetValue(4).ToString(),
                                // Age = Convert.ToInt32(reader.GetValue(5).ToString()),
                                // FatherName = reader.GetValue(6).ToString(),
                                SpouseName = reader.GetValue(7).ToString(),
                                // Occupation = reader.GetValue(8).ToString(),
                                // Location = reader.GetValue(9).ToString(),
                                // AddressLine1 = reader.GetValue(10).ToString(),
                                // AddressLine2 = reader.GetValue(11).ToString(),
                                // PostalCode = Convert.ToInt32(reader.GetValue(12).ToString()),
                                // Landmark = reader.GetValue(13).ToString(),
                                // HouseType = reader.GetValue(14).ToString(),
                                AlternateNo = reader.GetValue(15).ToString(),
                                CityId = city.Id,
                                StateId = city.StateId,

                                CreatedBy = user.UserName,
                                CreatedDate = DateTime.Now,
                                //Username = user.UserName,
                                EmpCode = user.EmployeeCode
                            };
                            enquiries.Add(enquiry);
                        }
                    }
                }
            }
            return enquiries;
        }

    }

    public class Response
    {
        public string message_id { get; set; }
        public int message_count { get; set; }
        public double price { get; set; }
    }
    public class RootObject
    {
        public Response Response { get; set; }
        public string ErrorMessage { get; set; }
        public int Status { get; set; }
    }
}

