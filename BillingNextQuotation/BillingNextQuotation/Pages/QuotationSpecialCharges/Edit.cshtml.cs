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
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.QuotationSpecialCharges
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class EditModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public EditModel(BillingNextQuotation.Data.ApplicationDbContext context, UserManager<QuotationGenUser> userManager, ILogger<EditModel> logger)
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
                .Include(q => q.SpecialCharges).FirstOrDefaultAsync(m => m.SpecialChargesId == specialchargeid && m.QuotationId== quotationid);

            if (QuotationSpecialCharges == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var quotationToUpdate = await _context.Quotations.FindAsync(QuotationSpecialCharges.QuotationId);
            var amountsum = Convert.ToSingle(Math.Floor(_context.QuotationDetails.Where(a => a.QuotationId.Equals(QuotationSpecialCharges.QuotationId)).Sum(a => a.ProductAmount)));
            if (!QuotationSpecialCharges.DefaultCalculationOverride)
            {
                var specialCharge = await _context.SpecialCharges.FindAsync(QuotationSpecialCharges.SpecialChargesId);
                if (specialCharge.SpecialChargeType == Enums.SpecialChargeType.FixedCharge)
                {
                    QuotationSpecialCharges.SpecialChargeAmount = Convert.ToSingle(Math.Floor(specialCharge.SpecialChargeFixedAmount));
                    QuotationSpecialCharges.DefaultCalculationOverride = false;
                }
                else
                {
                    if (specialCharge.SpecialChargeFixedAmount > 0)
                    {
                        float SCPercAmount = Convert.ToSingle(Math.Floor(Convert.ToSingle((specialCharge.SpecialChargePercentage * amountsum) / 100)));
                        if (SCPercAmount > specialCharge.SpecialChargeFixedAmount)
                        {
                            QuotationSpecialCharges.SpecialChargeAmount = specialCharge.SpecialChargeFixedAmount;
                            QuotationSpecialCharges.DefaultCalculationOverride = false;
                        }
                        else
                        {
                            QuotationSpecialCharges.SpecialChargeAmount = SCPercAmount;
                            QuotationSpecialCharges.DefaultCalculationOverride = false;
                        }
                    }
                    else
                    {
                        float SCPercAmount = Convert.ToSingle(Math.Floor(Convert.ToSingle((specialCharge.SpecialChargePercentage * amountsum) / 100)));
                        QuotationSpecialCharges.SpecialChargeAmount = SCPercAmount;
                        QuotationSpecialCharges.DefaultCalculationOverride = false;
                    }
                }
            }
            else
            {
                QuotationSpecialCharges.SpecialChargeAmount = QuotationSpecialCharges.SpecialChargeAmount;
                QuotationSpecialCharges.DefaultCalculationOverride = true;
            }


            try
            {
                _context.Attach(QuotationSpecialCharges);
                _context.Entry(QuotationSpecialCharges).Property("SpecialChargeAmount").IsModified = true;
                _context.Entry(QuotationSpecialCharges).Property("DefaultCalculationOverride").IsModified = true;
                await _context.SaveChangesAsync();
                
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
            catch (DbUpdateConcurrencyException)
            {
                if (!QuotationSpecialChargesExists(QuotationSpecialCharges.SpecialChargesId, QuotationSpecialCharges.QuotationId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch(Exception e)
            {
                ModelState.AddModelError(string.Empty, "Error occured: "+e.ToString());
                return Page();
            }
        }

        private bool QuotationSpecialChargesExists(string specialchargeid, string quotationid)
        {
            return _context.QuotationSpecialCharges.Any(e => e.SpecialChargesId == specialchargeid && e.QuotationId==quotationid);
        }
    }
}
