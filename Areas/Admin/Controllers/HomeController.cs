using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using UnitedSales.Areas.Identity.Data;
using UnitedSales.Models;

namespace UnitedSales.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger,
                            UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            IWebHostEnvironment webHostEnvironment,
                            ApplicationDbContext repository,
                            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _repository = repository;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
           
                ViewBag.Closed = "0";
                ViewBag.FllowUp = "0";
                ViewBag.NotInterest = "0";
                ViewBag.OpenLeads = "0";
                ViewBag.PendingSV = "0";
                ViewBag.CompleteSV = "0";
           
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PersonalProfile()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index");
            }

            ApplicationUserViewModel model = new ApplicationUserViewModel();

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
                model.User = user;

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult ManageEmployee()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditEmployee(string id = null)
        {
            if (id == null)
            {
                return NoContent();
            }

            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditEmployee(ApplicationUser model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var user = await _userManager.FindByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                if (model.Email == "" || model.Email == null)
                {
                    ModelState.AddModelError(string.Empty, "Please enter email address.");
                    return View(model);
                }

                if (user != null)
                {
                    //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //await _userManager.ResetPasswordAsync(user, token, "Abc123#$%");

                    if (user.EmployeeType != model.EmployeeType)
                    {
                        if (await _userManager.IsInRoleAsync(user, user.EmployeeType))
                            await _userManager.RemoveFromRoleAsync(user, user.EmployeeType);

                        await AddRoles(user, model.EmployeeType);
                        user.EmployeeType = model.EmployeeType;
                    }

                    if (user.Email != model.Email)
                    {
                        user.Email = model.Email;
                        user.EmailConfirmed = true;
                    }

                    if (user.UserName != model.UserName)
                    {
                        var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                        if (!setUserNameResult.Succeeded)
                        {
                            return View(model);
                        }
                    }

                    user.EmployeeCode = model.EmployeeCode;

                    user.PhoneNumber = model.PhoneNumber;
                    user.Designation = model.Designation;
                    user.DateofJoining = model.DateofJoining;
                    user.IsActive = model.IsActive;
                    user.Gender = model.Gender;
                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;
                    user.ChannelHeadId = model.ChannelHeadId;

                    await _repository.SaveChangesAsync();
                }

                return RedirectToAction("ManageEmployee", _userManager.Users);
            }

            return View(model);
        }

         public async Task AddRoles(ApplicationUser user, string RoleName)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = RoleName
            };

            if (!(await _roleManager.RoleExistsAsync(RoleName)))
            {
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    if (!(await _userManager.IsInRoleAsync(user, RoleName)))
                        await _userManager.AddToRoleAsync(user, RoleName);
                }
            }
            else
            {
                if (!(await _userManager.IsInRoleAsync(user, RoleName)))
                    await _userManager.AddToRoleAsync(user, RoleName);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (model.Avatar != null)
                    {
                        string filePath = user.ProfileImage == null ? null : Path.Combine(_webHostEnvironment.WebRootPath, "profileimages/", user.ProfileImage);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        user.ProfileImage = UploadedFile(model);
                    }

                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;
                    user.Gender = model.Gender;
                    user.DateOfBirth = model.DateOfBirth;
                    user.FatherName = model.FatherName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.IsActive = true;

                    if (user.PAN != model.PAN)
                    {
                        user.PAN = model.PAN;
                    }

                    if (user.AadhaarNo != model.AadhaarNo)
                    {
                        user.AadhaarNo = model.AadhaarNo;
                    }

                    if (user.HighestQualification != model.HighestQualification)
                    {
                        user.HighestQualification = model.HighestQualification;
                    }

                    await _repository.SaveChangesAsync();

                    CookieOptions option = new CookieOptions();
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "/profileimages/");

                    if (user.ProfileImage != null)
                        Response.Cookies.Append("userProf", path + user.ProfileImage, option);
                    else
                        Response.Cookies.Append("userProf", "", option);

                    if (user.Gender.ToLower() == "male")
                    {
                        Response.Cookies.Append("gender", "0", option);
                    }
                    else if (user.Gender.ToLower() == "female")
                    {
                        Response.Cookies.Append("gender", "1", option);
                    }
                    else
                    {
                        Response.Cookies.Append("gender", "1", option);
                    }
                }

                return RedirectToAction("Index");
            }

            return View(user);
        }

        private string UploadedFile(ApplicationUser model)
        {
            string uniqueFileName = null;

            if (model.Avatar != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "profileimages");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Avatar.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Avatar.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet, ActionName("DeleteEmployee")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteEmployee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{user.Id}'.");
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            return RedirectToAction("ManageEmployee");
        }
    }
}