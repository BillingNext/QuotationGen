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
    public class DetailsModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public DetailsModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.QuotationNote QuotationNote { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QuotationNote = await _context.QuotationNotes
                .Include(q => q.Company).FirstOrDefaultAsync(m => m.QuotationNoteId == id);

            if (QuotationNote == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
