using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.Areas.Identity.Pages.Account.Manage.Admin
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class IndexModel : PageModel
    {
        //private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public IndexModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            //   _context = context;
        }




        public async Task OnGetAsync()
        {

        }
    }

    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class IndexGridModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexGridModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<QuotationUser> QuotationUsers { get; set; }

        public async Task OnGetAsync()
        {
            QuotationUsers = (from ep in _context.UserRoles
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
                                  PhoneNumberConfirmed = u.PhoneNumberConfirmed
                              }).AsQueryable();
        }
    }
}
