using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.QuotationSpecialCharges
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class DetailsModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly ILogger<DetailsModel> _logger;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public DetailsModel(BillingNextQuotation.Data.ApplicationDbContext context, UserManager<QuotationGenUser> userManager, ILogger<DetailsModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public Models.QuotationSpecialCharges QuotationSpecialCharges { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QuotationSpecialCharges = await _context.QuotationSpecialCharges
                .Include(q => q.Quotation)
                .Include(q => q.SpecialCharges).FirstOrDefaultAsync(m => m.SpecialChargesId == id);

            if (QuotationSpecialCharges == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
