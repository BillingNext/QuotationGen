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

namespace BillingNextQuotation.QuotationNote
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class IndexModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public IndexModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.QuotationNote> QuotationNote { get;set; }

        public async Task OnGetAsync()
        {
            QuotationNote = await _context.QuotationNotes
                .Include(q => q.Company).ToListAsync();
        }
    }
}
