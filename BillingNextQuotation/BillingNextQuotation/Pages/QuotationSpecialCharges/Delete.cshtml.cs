using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.QuotationSpecialCharges
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class DeleteModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly ILogger<DeleteModel> _logger;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public DeleteModel(BillingNextQuotation.Data.ApplicationDbContext context, UserManager<QuotationGenUser> userManager, ILogger<DeleteModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        [BindProperty]
        public Models.QuotationSpecialCharges QuotationSpecialCharges { get; set; }

        public async Task<IActionResult> OnGetAsync(string specialchargeid, string quotationid)
        {
            if (specialchargeid == null || quotationid == null)
            {
                return NotFound();
            }

            QuotationSpecialCharges = await _context.QuotationSpecialCharges
                .Include(q => q.Quotation)
                .Include(q => q.SpecialCharges).FirstOrDefaultAsync(m => m.SpecialChargesId == specialchargeid && m.QuotationId == quotationid);

            if (QuotationSpecialCharges == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string specialchargeid, string quotationid)
        {
            if (specialchargeid == null || quotationid == null)
            {
                return NotFound();
            }

            QuotationSpecialCharges = await _context.QuotationSpecialCharges.FirstOrDefaultAsync(m => m.SpecialChargesId == specialchargeid && m.QuotationId == quotationid);

            if (QuotationSpecialCharges != null)
            {
                _context.QuotationSpecialCharges.Remove(QuotationSpecialCharges);
                await _context.SaveChangesAsync();
                var quotationToUpdate = await _context.Quotations.FindAsync(QuotationSpecialCharges.QuotationId);
                var amountsum = Convert.ToSingle(Math.Floor(_context.QuotationDetails.Where(a => a.QuotationId.Equals(QuotationSpecialCharges.QuotationId)).Sum(a => a.ProductAmount)));

                var user = await GetCurrentUserAsync();
                quotationToUpdate.QuotationGrandTotalAmount = _context.QuotationSpecialCharges.Where(q => q.QuotationId.Equals(QuotationSpecialCharges.QuotationId)).Sum(a => a.SpecialChargeAmount) + amountsum;
                quotationToUpdate.ModifiedByUserId = user.Id;
                quotationToUpdate.QuotationModificationDate = DateTime.Now;
                _context.Attach(quotationToUpdate);
                _context.Entry(quotationToUpdate).Property("QuotationGrandTotalAmount").IsModified = true;
                _context.Entry(quotationToUpdate).Property("ModifiedByUserId").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationModificationDate").IsModified = true;
                _context.SaveChanges();
                return Redirect($"/Quotation/Admin/Edit/{QuotationSpecialCharges.QuotationId}");
            }

            return NotFound();
        }
    }
}
