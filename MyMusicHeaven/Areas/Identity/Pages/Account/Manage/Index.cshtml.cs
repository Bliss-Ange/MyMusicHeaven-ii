using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMusicHeaven.Areas.Identity.Data;

namespace MyMusicHeaven.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<MyMusicHeavenUser> _userManager;
        private readonly SignInManager<MyMusicHeavenUser> _signInManager;

        public IndexModel(
            UserManager<MyMusicHeavenUser> userManager,
            SignInManager<MyMusicHeavenUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel //form structure 2 rules, first - show the user id, second - disabled the edit function for name and id section
        {
            
            [Required(ErrorMessage = "Fill in your phone No.")]
            [Display(Name = "Your Phone Number")]
            [StringLength(11, ErrorMessage = "Only input 10 - 11 phone numbers!", MinimumLength = 10)]
            public string PhoneNumber { get; set; }

            public string Full_Name { get; set; } //lock from edit

            public string User_id { get; set; }

            [Display(Name = "Your Age")]
            [Range(13, 100, ErrorMessage = "We only accept 13 and above years old user to be our member!")]
            public int Age { get; set; }


            [Display(Name = "Your Birthdate")]
            [DataType(DataType.Date)]
            public DateTime DOB { get; set; }


            [Display(Name = "Your address")]
            [RegularExpression(@"^[A-Z]+[a-zA-Z]*$", ErrorMessage = "Only capital letter in the first char, and only accept alpahabet.")]
            public string Address { get; set; }

            [Display(Name ="Profile Picture")]
            public byte[] ProfilePicture { get; set; }

        }

        private async Task LoadAsync(MyMusicHeavenUser user) //retrive data
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel //fetch data from table to form
            {
                PhoneNumber = phoneNumber,
                Full_Name = user.User_Full_Name,
                Age = user.User_Age,
                User_id = user.Id,
                DOB = user.User_DOB,
                Address = user.User_Address,
                ProfilePicture = user.ProfilePicture //display picture in the page
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid) //form have conflict or technical issue
            {
                await LoadAsync(user);
                StatusMessage = "Sorry technical isseu. We will fix it in soon.";
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.Address != user.User_Address)
            {
                user.User_Address = Input.Address;
            }

            if (Input.Age != user.User_Age)
            {
                user.User_Age = Input.Age;
            }

            if (Input.DOB != user.User_DOB)
            {
                user.User_DOB = Input.DOB;
            }

            await _userManager.UpdateAsync(user);

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
