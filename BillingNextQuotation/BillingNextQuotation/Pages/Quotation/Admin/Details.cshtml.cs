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

namespace BillingNextQuotation.Quotation.Admin
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class DetailsModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public DetailsModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
                .Include(q=>q.QuotationDetails)
                .Include(q => q.QuotationNote)
                .Include(q => q.QuotationSpecialCharges).ThenInclude(q => q.SpecialCharges).FirstOrDefaultAsync(m => m.QuotationId == id);

            if (Quotation == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostFinalize(string id)
        {
            var quotationToUpdate = await _context.Quotations.FindAsync(id);
            quotationToUpdate.QuotationStatus = Enums.QuotationStatus.Finalized;
            _context.Attach(quotationToUpdate);
            _context.Entry(quotationToUpdate).Property("QuotationStatus").IsModified = true;
            _context.SaveChanges();
            return RedirectToPage("/Quotation/Status/Finalize");
        }

        public async Task<IActionResult> OnPostNegotiation(string id)
        {
            var quotationToUpdate = await _context.Quotations.FindAsync(id);
            quotationToUpdate.QuotationStatus = Enums.QuotationStatus.Negotiation;
            _context.Attach(quotationToUpdate);
            _context.Entry(quotationToUpdate).Property("QuotationStatus").IsModified = true;
            _context.SaveChanges();
            return RedirectToPage("/Quotation/Status/Negotiate");
        }
        public async Task<IActionResult> OnPostUndiscard(string id)
        {
            var quotationToUpdate = await _context.Quotations.FindAsync(id);
            quotationToUpdate.QuotationStatus = Enums.QuotationStatus.Created;
            _context.Attach(quotationToUpdate);
            _context.Entry(quotationToUpdate).Property("QuotationStatus").IsModified = true;
            _context.SaveChanges();
            return Redirect($"/Quotation/Admin/Details/{id}");
        }
        public async Task<IActionResult> OnPostBilled(string id)
        {
            var quotationToUpdate = await _context.Quotations.FindAsync(id);
            quotationToUpdate.QuotationStatus = Enums.QuotationStatus.Billed;
            _context.Attach(quotationToUpdate);
            _context.Entry(quotationToUpdate).Property("QuotationStatus").IsModified = true;
            _context.SaveChanges();
            return Redirect($"/Quotation/Admin/Details/{id}");
        }


        public IActionResult OnPostDiscard(string id)
        {
            return Redirect("/Quotation/Admin/Delete/" + id);
        }
    }
}
