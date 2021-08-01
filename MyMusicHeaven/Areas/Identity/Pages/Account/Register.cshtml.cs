using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MyMusicHeaven.Areas.Identity.Data;
using MyMusicHeaven.Data;

namespace MyMusicHeaven.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<MyMusicHeavenUser> _signInManager;
        private readonly UserManager<MyMusicHeavenUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<MyMusicHeavenUser> userManager,
            SignInManager<MyMusicHeavenUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public SelectList RoleselectList = new SelectList //Role select by people who register
        (
        new List<SelectListItem>
        {
        new SelectListItem { Selected = true, Text = "Select Role", Value=""},
        new SelectListItem { Selected = false, Text = "Admin", Value="Admin"},
        new SelectListItem { Selected = false, Text = "Customer", Value="Customer"}
        }, "Value", "Text", 1);

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage ="Must give your full name before register.")]
            [StringLength(200, ErrorMessage ="Enter the name with 6-200 chars!", MinimumLength = 6)]
            [Display(Name ="Your Full Name.")]
            public string Full_Name { get; set; }

            [Required(ErrorMessage = "Fill in your age.")]
            [Display(Name ="Your Age")]
            [Range(13, 100, ErrorMessage ="We only accept 13 and above years old user to be our member!")]
            public int Age { get; set; }

            [Required(ErrorMessage = "Fill in your phone No.")]
            [Display(Name = "Your Phone Number")]
            [StringLength(11, ErrorMessage = "Only input 10 - 11 phone numbers!", MinimumLength = 10)]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Fill in your DOB.")]
            [Display(Name = "Your Birthdate")]
            [DataType(DataType.Date)]
            public DateTime DOB { get; set; }

            [Required(ErrorMessage = "Fill in your address.")]
            [Display(Name = "Your address")]
            [RegularExpression(@"^[A-Z]+[a-zA-Z]*$", ErrorMessage = "Only capital letter in the first char, and only accept alpahabet.")]
            public string Address { get; set; }


            [Display(Name = "What is your role?")]
            public string userRoles { get; set; }

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
            if (ModelState.IsValid) //from structure and data no problem, then proceed
            {
                var user = new MyMusicHeavenUser { 
                    UserName = Input.Email,
                    Email = Input.Email,
                    User_Full_Name = Input.Full_Name,
                    User_Age = Input.Age,
                    User_DOB = Input.DOB,
                    User_Address = Input.Address,
                    PhoneNumber = Input.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                
                var role = Roles.Customer.ToString(); //check role

                if(Input.userRoles == "Admin")
                {
                    role = Roles.Admin.ToString();
                }

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
