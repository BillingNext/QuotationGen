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

namespace BillingNextQuotation.SpecialCharges
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class DetailsModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public DetailsModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
            return Page();
        }
    }
}
