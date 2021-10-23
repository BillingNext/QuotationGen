using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.Areas.Identity.Pages.Account.Manage.Admin
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class DetailsModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly ILogger<DetailsModel> _logger;


        public DetailsModel(UserManager<QuotationGenUser> userManager, ILogger<DetailsModel> logger, BillingNextQuotation.Data.ApplicationDbContext context)
        {
             _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public QuotationUser QuotationUser { get; set; }

        public bool IsLockedOut { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QuotationUser = (from ep in _context.UserRoles
                             join u in _context.Users on ep.UserId equals u.Id
                             join r in _context.Roles on ep.RoleId equals r.Id
                             select new QuotationUser
                             {
                                 UserId = u.Id,
                                 RoleId = r.Id,
                                 Name = u.Name,
                                 RoleName = r.Name,
                                 PhoneNumber = u.PhoneNumber,
                                 UserName = u.UserName,
                                 CustomerCatagory = u.CustomerCatagory,
                                 PhysicalAddress = u.PhysicalAddress,
                                 ProfilePicture = u.ProfilePicture
                             }).FirstOrDefault(a => a.UserId == id);

            if (QuotationUser == null)
            {
                return NotFound();
            }
            IsLockedOut = await _userManager.IsLockedOutAsync(await _context.Users.FindAsync(id));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                var lockoutstatus = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now);
                if (lockoutstatus.Succeeded)
                {
                    _logger.LogInformation("User is now unlocked: " + user.UserName);
                    await _userManager.UpdateSecurityStampAsync(user);
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
