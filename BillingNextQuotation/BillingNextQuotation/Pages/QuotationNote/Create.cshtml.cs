using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.QuotationNote
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class CreateModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public CreateModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
            return Page();
        }

        [BindProperty]
        public Models.QuotationNote QuotationNote { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            QuotationNote.NoteCreationDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
                return Page();
            }
            var alreadydefault = _context.QuotationNotes.Where(a => a.IsNoteDefault == true).Any();
            if (alreadydefault && QuotationNote.IsNoteDefault)
            {
                ModelState.AddModelError(string.Empty, "A default note is already present. To make this note default, Edit the default note and from there select new default as this note.");
                ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
                return Page();
            }

            _context.QuotationNotes.Add(QuotationNote);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
