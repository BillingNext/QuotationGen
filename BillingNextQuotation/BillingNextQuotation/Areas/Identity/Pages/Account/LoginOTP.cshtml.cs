using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BillingNextQuotation.Data;
using BillingNextQuotation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BillingNextQuotation.Areas.Identity.Pages.Account
{
    public class LoginOTPModel : PageModel
    {
        private readonly UserManager<QuotationGenUser> userManager;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<LoginOTPModel> _logger;
        private readonly SignInManager<QuotationGenUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public LoginOTPModel(UserManager<QuotationGenUser> userManager, IMessageSender messageSender, ILogger<LoginOTPModel> logger, SignInManager<QuotationGenUser> signInManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            _messageSender = messageSender;
            _logger = logger;
            _signInManager = signInManager;
            _context = context;
        }
        
        public class InputModel
        {
            [Required(AllowEmptyStrings =false,ErrorMessage ="User Name is empty")]
            [Display(Name ="User Name")]
            public string UserName { get; set; }


            [StringLength(6, MinimumLength = 6, ErrorMessage = "Code should be 6 digit long.")]
            [Display(Name ="One Time Passowrd")]
            public string Code { get; set; }

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public byte[] ProfilePicture { get; set; }

        public bool UserValid { get; set; }

        public string ProfileName { get; set; }

        public Models.Company Company { get; set; }

        public IActionResult OnGet()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            UserValid = false;
            setCompany();
            return Page();
        }

        private void setCompany()
        {
            Company = _context.Companies.FirstOrDefault();
        }

        public async Task<IActionResult> OnPostUserName()
        {
            setCompany();
            if(!ModelState.IsValid)
            {
                return Page();
            }
            var user = await userManager.FindByNameAsync(Input.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name");
                _logger.LogInformation($"Unable to load user with user name: '{Input.UserName}' for login module.");
                return Page();
            }
            var roles=await userManager.GetRolesAsync(user);
            if (!roles.First().Equals("Dealers"))
            {
                return RedirectToPage("./Login");
            }
            if (!user.PhoneNumberConfirmed)
            {
                ModelState.AddModelError(string.Empty,"Your phone number is not yet confirmed! Wait till it gets confirmed.");
                return Page();
            }
            if (await userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("User account locked out: " + user.UserName);
                return RedirectToPage("./Lockout");
            }
            
            var code = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
          
            bool msg_success = await _messageSender.SendSmsAsync(user.PhoneNumber, "Hii, "+user.Name+". Your OTP for login is " + code);
            if (!msg_success)
            {
                _logger.LogError($"Error sending SMS occured for USER NAME: {Input.UserName}");
                return RedirectToPage("/Error");
            }
            else
            {
                _logger.LogInformation("OTP sent for "+ Input.UserName+" : "+ code);
            }

            ProfilePicture = user.ProfilePicture;
            UserValid = true;
            ProfileName = user.Name;
            return Page();

        }
        public async Task<IActionResult> OnPostLogin()
        {
            setCompany();
            if (String.IsNullOrEmpty(Input.Code) || String.IsNullOrWhiteSpace(Input.Code))
            {
                ModelState.AddModelError(string.Empty, "Please provide a code received in OTP");
                return Page();
            }
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Failed login attempt");
                return Page();
            }

            var user = await userManager.FindByNameAsync(Input.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User Name");
                _logger.LogInformation($"Unable to load user with user name: '{Input.UserName}' for login module.");
                return Page();
            }
            var auth_success = await userManager.VerifyChangePhoneNumberTokenAsync(user,Input.Code,user.PhoneNumber);

            if(auth_success)
            {
                if(await userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("User account locked out: "+user.UserName);
                    return RedirectToPage("./Lockout");
                }
                await _signInManager.SignInAsync(user, true, authenticationMethod: "OTP based login");
                return RedirectToPage("/Dashboard/GenUser/Index");
            }
            ModelState.AddModelError(string.Empty, "OTP entered is incorrect");
            return Page();
        }
    }
}
