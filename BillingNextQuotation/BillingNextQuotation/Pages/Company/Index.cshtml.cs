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

namespace BillingNextQuotation.Company
{
    [Authorize(Roles = "SuperAdmin,Developer")]
    public class IndexModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public IndexModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Company> Company { get;set; }

        public async Task OnGetAsync()
        {
            Company = await _context.Companies.ToListAsync();
        }
    }
}
