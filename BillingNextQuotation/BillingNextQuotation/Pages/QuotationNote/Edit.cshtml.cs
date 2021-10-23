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
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.QuotationNote
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class EditModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public EditModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
            if (QuotationNote.IsNoteDefault)
            {
                ViewData["QuotationNotes"] = new SelectList(_context.QuotationNotes.Where(a => a.IsNoteDefault == false), "QuotationNoteId", "NoteName");
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
          
            _context.Attach(QuotationNote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                if (!QuotationNote.IsNoteDefault)
                {
                    string defaultnoteid = Request.Form["quotationnotedefault"];
                    var QuotationNoteToUpdate =await _context.QuotationNotes.FirstOrDefaultAsync(m => m.QuotationNoteId == defaultnoteid);
                    QuotationNoteToUpdate.IsNoteDefault = true;
                    _context.Attach(QuotationNoteToUpdate);
                    _context.Entry(QuotationNoteToUpdate).Property("IsNoteDefault").IsModified = true;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuotationNoteExists(QuotationNote.QuotationNoteId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool QuotationNoteExists(string id)
        {
            return _context.QuotationNotes.Any(e => e.QuotationNoteId == id);
        }
    }
}
