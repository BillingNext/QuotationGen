using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BillingNextQuotation.Data;
using BillingNextQuotation.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BillingNextQuotation.Areas.Identity.Pages.Account
{
    public class VerifyPhoneModel : PageModel
    {
        private readonly UserManager<QuotationGenUser> userManager;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<VerifyPhoneModel> _logger;

        public VerifyPhoneModel(UserManager<QuotationGenUser> userManager, ILogger<VerifyPhoneModel> logger ,IMessageSender messageSender)
        {
            _logger = logger;
            this.userManager = userManager;
            _messageSender = messageSender;
        }

        public class InputModel
        {
            [Required]
            [StringLength(6,MinimumLength =6,ErrorMessage ="Code should be 6 digit long.")]
            public string Code { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var user = await userManager.FindByIdAsync(id);
            if(user==null)
            {
                return NotFound("User with userid "+id+" not found.");
            }
            if(user.PhoneNumberConfirmed)
            {
                return NotFound("Phone Number already confirmed");
            }
            var code = await userManager.GenerateChangePhoneNumberTokenAsync(user,user.PhoneNumber);
            var result= await _messageSender.SendSmsAsync(user.PhoneNumber, $"Your one time code for phone number confirmation is {code}.");
            if(result)
            {
                _logger.LogInformation("OTP sent for "+ user.UserName+ " : "+code);
                return Page();
            }
            return RedirectToPage("/Error");
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if(!ModelState.IsValid)
            {
                return Page();
            }
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User with userid " + id + " not found.");
            }
           
            var result= await userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, Input.Code);
            if(result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            ModelState.AddModelError(string.Empty,"Invalid Code");
            return Page();
        }
    }
}
