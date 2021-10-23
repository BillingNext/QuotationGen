using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.QuotationSpecialCharges
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class CreateModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly ILogger<CreateModel> _logger;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public CreateModel(BillingNextQuotation.Data.ApplicationDbContext context, UserManager<QuotationGenUser> userManager, ILogger<CreateModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        public IActionResult OnGet(string id)
        {
            var quotationspecialcharges = _context.QuotationSpecialCharges.Where(a => a.QuotationId.Equals(id)).ToList();
            if (quotationspecialcharges.Any())
            {
                var listofchargesExcpt = quotationspecialcharges.Select(a => a.SpecialChargesId).ToList();
                ViewData["SpecialCharges"] = new SelectList(_context.SpecialCharges.Where(q => !listofchargesExcpt.Contains(q.SpecialChargesId)), "SpecialChargesId", "SpecialChargeName");
            }
            Quotation = _context.Quotations.Where(a => a.QuotationId.Equals(id)).FirstOrDefault();
            return Page();
        }

        [BindProperty]
        public Models.QuotationSpecialCharges QuotationSpecialCharges { get; set; }

        public Models.Quotation Quotation { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var quotationToUpdate = await _context.Quotations.FindAsync(QuotationSpecialCharges.QuotationId);
                var amountsum = Convert.ToSingle(Math.Floor(_context.QuotationDetails.Where(a => a.QuotationId.Equals(QuotationSpecialCharges.QuotationId)).Sum(a => a.ProductAmount)));
                if (!QuotationSpecialCharges.DefaultCalculationOverride)
                {
                    var specialCharge = await _context.SpecialCharges.FindAsync(QuotationSpecialCharges.SpecialChargesId);
                    if (specialCharge.SpecialChargeType == Enums.SpecialChargeType.FixedCharge)
                    {
                        _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = QuotationSpecialCharges.QuotationId, SpecialChargesId = QuotationSpecialCharges.SpecialChargesId, SpecialChargeAmount = Convert.ToSingle(Math.Floor(specialCharge.SpecialChargeFixedAmount)), CompanyId = QuotationSpecialCharges.CompanyId, DefaultCalculationOverride = false });
                    }
                    else
                    {
                        if (specialCharge.SpecialChargeFixedAmount > 0)
                        {
                            float SCPercAmount = Convert.ToSingle(Math.Floor(Convert.ToSingle((specialCharge.SpecialChargePercentage * amountsum) / 100)));
                            if (SCPercAmount > specialCharge.SpecialChargeFixedAmount)
                            {
                                _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = QuotationSpecialCharges.QuotationId, SpecialChargesId = specialCharge.SpecialChargesId, SpecialChargeAmount = specialCharge.SpecialChargeFixedAmount, CompanyId = QuotationSpecialCharges.CompanyId, DefaultCalculationOverride = false });
                            }
                            else
                            {
                                _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = QuotationSpecialCharges.QuotationId, SpecialChargesId = specialCharge.SpecialChargesId, SpecialChargeAmount = SCPercAmount, CompanyId = QuotationSpecialCharges.CompanyId, DefaultCalculationOverride = false });
                            }
                        }
                        else
                        {
                            float SCPercAmount = Convert.ToSingle(Math.Floor(Convert.ToSingle((specialCharge.SpecialChargePercentage * amountsum) / 100)));
                            _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = QuotationSpecialCharges.QuotationId, SpecialChargesId = specialCharge.SpecialChargesId, SpecialChargeAmount = SCPercAmount, CompanyId = QuotationSpecialCharges.CompanyId, DefaultCalculationOverride = false });
                        }
                    }
                }
                else
                {
                    _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = QuotationSpecialCharges.QuotationId, SpecialChargesId = QuotationSpecialCharges.SpecialChargesId, SpecialChargeAmount = QuotationSpecialCharges.SpecialChargeAmount, CompanyId = QuotationSpecialCharges.CompanyId, DefaultCalculationOverride = true });
                }
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
            catch(Exception e)
            {
                var quotationspecialcharges = _context.QuotationSpecialCharges.Where(a => a.QuotationId.Equals(QuotationSpecialCharges.QuotationId)).ToList();
                if (quotationspecialcharges.Any())
                {
                    var listofchargesExcpt = quotationspecialcharges.Select(a => a.SpecialChargesId).ToList();
                    ViewData["SpecialCharges"] = new SelectList(_context.SpecialCharges.Where(q => !listofchargesExcpt.Contains(q.SpecialChargesId)), "SpecialChargesId", "SpecialChargeName");
                }
                Quotation = _context.Quotations.Where(a => a.QuotationId.Equals(QuotationSpecialCharges.QuotationId)).FirstOrDefault();
                ModelState.AddModelError(string.Empty, "Error occured: " + e.ToString());
                return Page();
            }
        }
    }
}
