using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using UnitedSales.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace UnitedSales.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter username")]
            public string Username { get; set; }

            //[Required(ErrorMessage = "Please enter email address / username")]
            // [EmailAddress]
            // public string Email { get; set; }

            [Required(ErrorMessage = "Please enter password.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/Admin");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin");

            if (ModelState.IsValid)
            {
                if (await _userManager.FindByNameAsync(Input.Username) != null)
                {
                    Input.Username = (await _userManager.FindByNameAsync(Input.Username)).UserName;
                }
                else if (await _userManager.FindByEmailAsync(Input.Username) != null)
                {
                    Input.Username = (await _userManager.FindByEmailAsync(Input.Username)).UserName;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Input.Username + " not found in our database, please sign up.");
                    return Page();
                }

                var user = await _userManager.FindByNameAsync(Input.Username);

                if (user != null && !user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your ID has been discontinued.");
                    return Page();
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    CookieOptions option = new CookieOptions();
                    var roles = await _userManager.GetRolesAsync(user);

                    foreach (string role in roles)
                    {
                        Response.Cookies.Append("role", role, option);
                    }

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


                    Response.Cookies.Append("fullName", user.FullName, option);

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
