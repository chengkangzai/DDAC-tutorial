using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewNewTry.Areas.Identity.Data;

namespace NewNewTry.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Please add the full name first before proceed with registration")]
            [Display(Name = "Full Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} Characters long.")]
            public string CustomerName { get; set; }

            [Required]
            [Display(Name = "Age")]
            public int CustomerAge { get; set; }

            [Required]
            [Display(Name = "DOB")]
            [DataType(DataType.Date)]
            public DateTime CustomerDOB { get; set; }

            [Display(Name = "Address")]
            [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "The address should start with capital letter")]
            public string CustomerLivingState { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                CustomerName = user.CustomerName,
                CustomerAge = user.CustomerAge,
                CustomerDOB = user.CustomerDOB,
                CustomerLivingState = user.CustomerLivingState
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

            if (!ModelState.IsValid)
            {
                // await LoadAsync(user);
                // return Page();
                return BadRequest("The form is not valid! Please check the fields and try again.");
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

            if (Input.CustomerName != user.CustomerName)
            {
                user.CustomerName = Input.CustomerName;
            }
            if (Input.CustomerAge != user.CustomerAge)
            {
                user.CustomerAge = Input.CustomerAge;
            }

            if (Input.CustomerDOB != user.CustomerDOB)
            {
                user.CustomerDOB = Input.CustomerDOB;
            }

            if (Input.CustomerLivingState != user.CustomerLivingState)
            {
                user.CustomerLivingState = Input.CustomerLivingState;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to update your profile.";
                return RedirectToPage();
            }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
