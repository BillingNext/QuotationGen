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

namespace BillingNextQuotation.Quotation
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public DeleteModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Quotation Quotation { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Quotation = await _context.Quotations
                .Include(q => q.Company)
                .Include(q => q.QuotationGenUser)
                .Include(q => q.QuotationNote).FirstOrDefaultAsync(m => m.QuotationId == id);

            if (Quotation == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Quotation = await _context.Quotations.FindAsync(id);

            if (Quotation != null)
            {
                Quotation.QuotationStatus = Enums.QuotationStatus.Discarded;
                _context.Attach(Quotation);
                _context.Entry(Quotation).Property("QuotationStatus").IsModified = true;
                _context.SaveChanges();
            }

            return RedirectToPage("./Index");
        }
    }
}
