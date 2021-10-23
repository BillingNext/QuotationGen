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

namespace BillingNextQuotation.SpecialCharges
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
        ViewData["Company"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
            return Page();
        }

        [BindProperty]
        public Models.SpecialCharges SpecialCharges { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (SpecialCharges.SpecialChargeType == Enums.SpecialChargeType.FixedCharge)
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
                if (SpecialCharges.SpecialChargePercentage == 0 || SpecialCharges.SpecialChargePercentage == null)
                {
                    ModelState.AddModelError(string.Empty, "Percentage amount should be more than zero.");
                    ViewData["Company"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
                    return Page();
                }
            }
            _context.SpecialCharges.Add(SpecialCharges);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
