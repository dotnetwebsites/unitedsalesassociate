using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using UnitedSales.Areas.Identity.Data;

namespace UnitedSales.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter email address")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            // [Required(ErrorMessage = "Please enter password")]
            // [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            // [DataType(DataType.Password)]
            // [Display(Name = "Password")]
            // public string Password { get; set; }

            // [DataType(DataType.Password)]
            // [Display(Name = "Confirm password")]
            // [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            // public string ConfirmPassword { get; set; }
            
            public string Password
            {
                get
                {
                    return "Abc123#$%";
                }
            }

            [Required(ErrorMessage = "Please enter first name")]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string Firstname { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Middle Name")]
            public string Middlename { get; set; }

            [Required(ErrorMessage = "Please enter last name")]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "Required mobile no")]
            [MaxLength(10)]
            [MinLength(10, ErrorMessage = "Mobile no must be 10-digit without prefix")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
            [Display(Name = "Mobile No")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Please enter Username")]
            [DataType(DataType.Text)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            public string Gender { get; set; }

            public string Designation { get; set; }

            [Required(ErrorMessage = "Please enter employee code")]
            public string EmployeeCode { get; set; }

            [Required(ErrorMessage = "Please select user")]
            public string EmployeeType { get; set; }

            [Required(ErrorMessage = "Please enter Date of Joining")]
            [DataType(DataType.Date)]
            public DateTime DateOfJoining { get; set; }

            [Required(ErrorMessage = "Please enter Date of Birth")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            public string Address { get; set; }

            public string HighestQualification { get; set; }
            public IFormFile ProfileImage { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    EmployeeType = Input.EmployeeType,
                    EmployeeCode = Input.EmployeeCode,
                    FirstName = Input.Firstname,
                    MiddleName = Input.Middlename,
                    LastName = Input.Lastname,
                    PhoneNumber = Input.PhoneNumber,
                    Designation = Input.Designation,
                    DateofJoining = Input.DateOfJoining,
                    Gender = Input.Gender,
                    Address = Input.Address,
                    HighestQualification = Input.HighestQualification,

                    UserName = Input.Username,
                    Email = Input.Email,
                    //ChannelHeadId = Input.ChannelHeadId,
                    //ProfileImage = Input.ProfileImage == null ? null : UploadedFile(Input.ProfileImage),
                    IsActive = true
                };

                var codeExists = _userManager.Users.Any(p => p.EmployeeCode == Input.EmployeeCode);
                var emailExists = _userManager.Users.Any(p => p.Email == Input.Email);

                if (codeExists)
                {
                    ModelState.AddModelError(string.Empty, "Employee code already exists.");
                    return Page();
                }

                if (emailExists)
                {
                    ModelState.AddModelError(string.Empty, "Email already exists.");
                    return Page();
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await AddRoles(user, Input.EmployeeType);

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

                    //if we need to confirm email manually
                    await _userManager.ConfirmEmailAsync(user, code);

                    // string htmlBody = @"<div style='font-family: Segoe UI;font-size: small;'>
                    //                     <strong>Dear " + user.FullName + @",</strong>
                    //                     <p>Greeting from EESHA Infra.</p>
                    //                     <p>Welcome to EESHA Infra Housing, Property Enquiry web portal that you can signin accesses with your </p>
                    //                     <p>Email ID: " + user.Email + @"</p>
                    //                     <p>Password: Abc123#$%</p>
                    //                     <p>Please click on URL: <a href='https://eesha.pixal.in'>https://eesha.pixal.in/</a></p>
                    //                     <p>Once you log in, you can access employee information.</p>
                    //                     <p><strong>Regards,</strong></p>
                    //                     <p><strong>EESHA Infra Housing Pvt. Ltd.</strong></p>
                    //                     </div>";

                    //_emailSender.SendEmail(Mail.DNR, Input.Email, user.FullName, "Your EESHA Infra Housing account has been created.", htmlBody);

                    StatusMessage = "Employee successfully added.";

                    return RedirectToAction("ManageEmployee", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // if (ModelState.IsValid)
            // {
            //     var user = new ApplicationUser
            //     {
            //         EmployeeType = Input.EmployeeType,
            //         EmployeeCode = Input.EmployeeCode,
            //         FirstName = Input.Firstname,
            //         MiddleName = Input.Middlename,
            //         LastName = Input.Lastname,
            //         PhoneNumber = Input.PhoneNumber,
            //         Designation = Input.Designation,
            //         DateofJoining = DateTime.Now, //Input.DateOfJoining,
            //         Gender = Input.Gender,
            //         Address = Input.Address,
            //         HighestQualification = Input.HighestQualification,

            //         UserName = Input.Username,
            //         Email = Input.Email,
            //         ProfileImage = Input.ProfileImage == null ? null : UploadedFile(Input.ProfileImage),
            //         IsActive = true
            //     };
            //     var result = await _userManager.CreateAsync(user, Input.Password);
            //     if (result.Succeeded)
            //     {
            //         _logger.LogInformation("User created a new account with password.");

            //         var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //         code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //         var callbackUrl = Url.Page(
            //             "/Account/ConfirmEmail",
            //             pageHandler: null,
            //             values: new { area = "Identity", userId = user.Id, code = code },
            //             protocol: Request.Scheme);

            //         await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
            //             $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            //         if (_userManager.Options.SignIn.RequireConfirmedAccount)
            //         {
            //             return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
            //         }
            //         else
            //         {
            //             await _signInManager.SignInAsync(user, isPersistent: false);
            //             return LocalRedirect(returnUrl);
            //         }
            //     }
            //     foreach (var error in result.Errors)
            //     {
            //         ModelState.AddModelError(string.Empty, error.Description);
            //     }
            // }

            // If we got this far, something failed, redisplay form
            return Page();
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

        private string UploadedFile(IFormFile model)
        {
            string uniqueFileName = null;

            if (model != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "profileimages");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
