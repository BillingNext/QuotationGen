using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BillingNextQuotation.Quotation.Admin
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
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

            Quotation = await _context.Quotations.FirstOrDefaultAsync(m => m.QuotationId == id);

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
                _context.Quotations.Remove(Quotation);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Quotation/Admin/Index");
        }

        public async Task<IActionResult> OnPostDiscardAsync(string id)
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

            return RedirectToPage("/Quotation/Admin/Index");
        }
    }
}