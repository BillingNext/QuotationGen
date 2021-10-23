using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BillingNextQuotation.Data;
using BillingNextQuotation.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BillingNextQuotation.Areas.Identity.Pages.Account.Manage.Admin
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class VerifyPhoneChangeModel : PageModel
    {
        private readonly UserManager<QuotationGenUser> userManager;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<VerifyPhoneChangeModel> _logger;
        private readonly SignInManager<QuotationGenUser> _signInManager;

        public VerifyPhoneChangeModel(UserManager<QuotationGenUser> userManager, ILogger<VerifyPhoneChangeModel> logger, IMessageSender messageSender, SignInManager<QuotationGenUser> signInManager)
        {
            _logger = logger;
            this.userManager = userManager;
            _messageSender = messageSender;
            _signInManager = signInManager;
        }

        public class InputModel
        {
            [Required]
            [StringLength(6, MinimumLength = 6, ErrorMessage = "Code should be 6 digit long.")]
            public string Code { get; set; }

            [Required]
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; }
        }

        public string PhoneNumber { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string phoneNumber)
        {
            if (id == null || phoneNumber == null)
            {
                return NotFound();
            }
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User with userid " + id + " not found.");
            }
            string code = await userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            _messageSender.SendSmsAsync(phoneNumber, $"Phone Number Change Request OTP. Your OTP for Phone Number Change is: {code}");
            _logger.LogInformation("Phone Number Change Request OTP: " + code);
            PhoneNumber = phoneNumber;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User with userid " + id + " not found.");
            }

            var result = await userManager.ChangePhoneNumberAsync(user, Input.PhoneNumber, Input.Code);

            if (result.Succeeded)
            {
                _messageSender.SendSmsAsync(Input.PhoneNumber, "Your new phone number have been verified. You'll receive OTPs on this new number now.");
                await userManager.UpdateSecurityStampAsync(user);
                return Redirect("/Identity/Account/Manage/Admin/Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid Code");
            return Page();
        }
    }
}
