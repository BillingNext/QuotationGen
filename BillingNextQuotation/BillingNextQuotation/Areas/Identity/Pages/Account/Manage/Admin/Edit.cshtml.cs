using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;
using BillingNextQuotation.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.Areas.Identity.Pages.Account.Manage.Admin
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class EditModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly IMessageSender _messageSender;
        private readonly ILogger<EditModel> _logger;
        private readonly SignInManager<QuotationGenUser> _signInManager;

        public EditModel(BillingNextQuotation.Data.ApplicationDbContext context, UserManager<QuotationGenUser> userManager, ILogger<EditModel> logger, IMessageSender messageSender, SignInManager<QuotationGenUser> signInManager)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _messageSender = messageSender;
        }

        [BindProperty]
        public QuotationUser QuotationUser { get; set; }

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
                                 ProfilePicture = u.ProfilePicture,
                                 PhoneNumberConfirmed = u.PhoneNumberConfirmed
                             }).FirstOrDefault(a => a.UserId == id);

            if (QuotationUser == null)
            {
                return NotFound();
            }
            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {

            var profilephoto_updated = Convert.ToBoolean(Request.Form["profpicupdated"]);
            bool phonenumber_updated = _context.Users.Where(a => a.Id.Equals(QuotationUser.UserId)).Select(a => a.PhoneNumber).First() == QuotationUser.PhoneNumber ? false : true;
            bool username_updated = _context.Users.Where(a => a.Id.Equals(QuotationUser.UserId)).Select(a => a.UserName).First() == QuotationUser.UserName ? false : true;
            bool canChangeUsername = true;
            if (username_updated)
            {
                canChangeUsername = !(_context.Users.Where(a => a.UserName.Equals(QuotationUser.UserName)).Any());
            }
            if (!canChangeUsername)
            {
                ModelState.AddModelError(string.Empty, "User Name is already taken");
                ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");
                return Page();
            }
            var userToUpdate = await _context.Users.FindAsync(QuotationUser.UserId);
            _context.Attach(userToUpdate);

            if (canChangeUsername && username_updated)
            {
                userToUpdate.UserName = QuotationUser.UserName;
                _context.Entry(userToUpdate).Property("UserName").IsModified = true;
                userToUpdate.Email = QuotationUser.UserName + "@bnquotation.sys";
                _context.Entry(userToUpdate).Property("Email").IsModified = true;
                userToUpdate.NormalizedEmail = QuotationUser.UserName + "@bnquotation.sys".ToUpper();
                _context.Entry(userToUpdate).Property("NormalizedEmail").IsModified = true;
                userToUpdate.NormalizedUserName = QuotationUser.UserName.ToUpper();
                _context.Entry(userToUpdate).Property("NormalizedUserName").IsModified = true;
            }


            userToUpdate.Name = QuotationUser.Name;
            userToUpdate.PhysicalAddress = QuotationUser.PhysicalAddress;
            _context.Entry(userToUpdate).Property("Name").IsModified = true;
            _context.Entry(userToUpdate).Property("PhysicalAddress").IsModified = true;

            if (QuotationUser.RoleName != "Dealers")
            {
                userToUpdate.CustomerCatagory = BillingNextQuotation.Enums.CustomerCategory.NotApplicable;
                _context.Entry(userToUpdate).Property("CustomerCatagory").IsModified = true;
            }
            else
            {
                if (userToUpdate.CustomerCatagory == BillingNextQuotation.Enums.CustomerCategory.NotApplicable)
                {
                    userToUpdate.CustomerCatagory = BillingNextQuotation.Enums.CustomerCategory.CategoryB;
                }
                else
                {
                    userToUpdate.CustomerCatagory = QuotationUser.CustomerCatagory;
                }
                _context.Entry(userToUpdate).Property("CustomerCatagory").IsModified = true;
            }


            if (!_context.UserRoles.Where(a => a.UserId.Equals(QuotationUser.UserId) && a.RoleId.Equals(QuotationUser.RoleId)).Any())
            {
                await _userManager.RemoveFromRoleAsync(userToUpdate, QuotationUser.RoleName);
                await _userManager.AddToRoleAsync(userToUpdate, _context.Roles.Where(a => a.Id.Equals(QuotationUser.RoleId)).Select(a => a.Name).FirstOrDefault());
            }

            if (profilephoto_updated)
            {
                if (QuotationUser.ProfilePic != null)
                {
                    if (QuotationUser.ProfilePic.Length > 0)

                    //Convert Image to byte and save to database

                    {
                        byte[] p1 = null;
                        using (var fs1 = QuotationUser.ProfilePic.OpenReadStream())
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                        userToUpdate.ProfilePicture = p1;
                        _context.Entry(userToUpdate).Property("ProfilePicture").IsModified = true;
                    }
                }
            }



            try
            {
                await _context.SaveChangesAsync();
                await _userManager.UpdateSecurityStampAsync(userToUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(QuotationUser.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (phonenumber_updated)
            {
                return Redirect($"/Identity/Account/Manage/Admin/VerifyPhoneChange/{QuotationUser.UserId}/{QuotationUser.PhoneNumber}");
            }
            return RedirectToPage("./Index");
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
