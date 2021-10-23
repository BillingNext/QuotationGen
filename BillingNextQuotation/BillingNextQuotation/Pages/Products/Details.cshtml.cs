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

namespace BillingNextQuotation.Products
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class DetailsModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public DetailsModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Products Products { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Products = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);

            if (Products == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
