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

namespace BillingNextQuotation.SpecialCharges
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
        public Models.SpecialCharges SpecialCharges { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SpecialCharges = await _context.SpecialCharges
                .Include(s => s.Company).FirstOrDefaultAsync(m => m.SpecialChargesId == id);

            if (SpecialCharges == null)
            {
                return NotFound();
            }
           ViewData["Company"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(SpecialCharges.SpecialChargeType== Enums.SpecialChargeType.FixedCharge)
            {
                if (SpecialCharges.SpecialChargeFixedAmount == 0 || SpecialCharges.SpecialChargeFixedAmount == null)
                {
                    ModelState.AddModelError(string.Empty, "Fixed amount should be more than zero.");
                    ViewData["Company"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
                    return Page();
                }
                SpecialCharges.SpecialChargePercentage = null;
            }
            else
            {
                if(SpecialCharges.SpecialChargePercentage == 0 || SpecialCharges.SpecialChargePercentage==null)
                {
                    ModelState.AddModelError(string.Empty, "Percentage amount should be more than zero.");
                    ViewData["Company"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
                    return Page();
                }
            }
            _context.Attach(SpecialCharges).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialChargesExists(SpecialCharges.SpecialChargesId))
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

        private bool SpecialChargesExists(string id)
        {
            return _context.SpecialCharges.Any(e => e.SpecialChargesId == id);
        }
    }
}
