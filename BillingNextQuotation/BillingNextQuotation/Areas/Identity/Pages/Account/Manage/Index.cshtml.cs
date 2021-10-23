using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BillingNextQuotation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BillingNextQuotation.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly SignInManager<QuotationGenUser> _signInManager;
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public IndexModel(
            UserManager<QuotationGenUser> userManager,
            SignInManager<QuotationGenUser> signInManager,
            BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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

            [Display(Name = "Name")]
            [Required(ErrorMessage = "Name can not be Null")]
            public string Name { get; set; }


            [Display(Name = "Physical Address")]
            [DataType(DataType.MultilineText)]
            [Required(ErrorMessage = "Physical Address can not be Null")]
            public string PhysicalAddress { get; set; }
        }

        private async Task LoadAsync(QuotationGenUser user)
        {

            Input = new InputModel
            {
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                PhysicalAddress = user.PhysicalAddress
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
                await LoadAsync(user);
                return Page();
            }
            _context.Attach(user);
            user.Name = Input.Name;
            user.PhysicalAddress = Input.PhysicalAddress;
            _context.Entry(user).Property("Name").IsModified = true;
            _context.Entry(user).Property("PhysicalAddress").IsModified = true;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception)
            {
                StatusMessage = "Problem occured updating profile Information";
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                    return Redirect($"/Identity/Account/Manage/Admin/VerifyPhoneChange/{user.Id}/{Input.PhoneNumber}");

            }

            return RedirectToPage();
        }
    }
}
