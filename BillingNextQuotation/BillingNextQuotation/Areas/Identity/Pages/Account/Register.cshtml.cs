using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BillingNextQuotation.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BillingNextQuotation.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "SuperAdmin,Developer")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<QuotationGenUser> _signInManager;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<QuotationGenUser> userManager,
            SignInManager<QuotationGenUser> signInManager,
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

        public class InputModel
        {
            [Required(ErrorMessage ="Name can not be null.")]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "User Name (This will be used for login)")]
            [RegularExpression("^[a-zA-Z0-9_-]*$", ErrorMessage = "Username format is incorrect.")]
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Customer Category")]
            public Enums.CustomerCategory CustomerCategory { get; set; }

            [Required]
            [Display(Name = "Role")]
            public Enums.Roles Roles { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            [StringLength(10,MinimumLength =10, ErrorMessage ="Enter 10 digit phone number")]
            [RegularExpression("^[0-9]*$",ErrorMessage ="Invalid Phone Number format.")]
            [Required(ErrorMessage ="Phone Number is empty.")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Please Specify Address"), StringLength(200)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Address")]
            public string PhysicalAddress { get; set; }

            public byte[] ProfilePicture { get; set; }

            [DataType(DataType.Upload)]
            [Display(Name = "User Profile Picture")]
            public IFormFile ProfilePic { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
         
            if (ModelState.IsValid)
            {
                if (Input.ProfilePic != null)
                {
                    if (Input.ProfilePic.Length > 0)
                    //Convert Image to byte and save to database
                    {
                        byte[] p1 = null;
                        using (var fs1 = Input.ProfilePic.OpenReadStream())
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                        Input.ProfilePicture = p1;
                    }
                }
                else
                {
                    Input.ProfilePicture = null;
                }
                var user = new QuotationGenUser { 
                    UserName = Input.UserName, 
                    Email = Input.UserName+"@bnquotation.sys",
                    Name= Input.Name,
                    CustomerCatagory= Input.CustomerCategory,
                    PhoneNumber=Input.PhoneNumber,
                    ProfilePicture=Input.ProfilePicture,
                    PhysicalAddress= Input.PhysicalAddress
                };
                if(Input.Roles==Enums.Roles.SuperAdmin || Input.Roles==Enums.Roles.Assistant)
                {
                    Input.CustomerCategory = Enums.CustomerCategory.NotApplicable;
                }
                if (Input.Roles == Enums.Roles.Dealers && Input.CustomerCategory == Enums.CustomerCategory.NotApplicable)
                {
                    ModelState.AddModelError(string.Empty, "Dealer must have a Category");
                }
                else
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");
                        await _userManager.AddToRoleAsync(user, Input.Roles.ToString());
                        return RedirectToPage("./VerifyPhone",new { id = user.Id });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
               
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
