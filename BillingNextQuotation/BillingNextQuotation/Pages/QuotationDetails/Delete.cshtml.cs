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
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BillingNextQuotation.QuotationDetails
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
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public Models.QuotationDetails QuotationDetails { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QuotationDetails = await _context.QuotationDetails.Include(a=>a.Quotation).FirstOrDefaultAsync(m => m.QuotationDetailsId == id);
            if(_context.QuotationDetails.Where(a=>a.QuotationId.Equals(QuotationDetails.QuotationId)).Count()==1)
            {
                firstRowOnlyFlag = 1;
            }
            if (QuotationDetails == null)
            {
                return NotFound();
            }
            return Page();
        }

        public int firstRowOnlyFlag { get; set; }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QuotationDetails = await _context.QuotationDetails.FindAsync(id);

            if (QuotationDetails != null)
            {
                _context.QuotationDetails.Remove(QuotationDetails);
                await _context.SaveChangesAsync();
            }
            await reCalculateSpecialChargesAsync(QuotationDetails.QuotationId);
            reCalculateSequenceNumber(QuotationDetails.QuotationId);
            var flag=Convert.ToInt32(Request.Form["flag"]);
            if(flag==1)
            {
                return Redirect($"/QuotationDetails/Create/{QuotationDetails.QuotationId}");
            }
            return Redirect($"/Quotation/Admin/Edit/{QuotationDetails.QuotationId}");
        }

        private void reCalculateSequenceNumber(string quotationid)
        {
            var quotationDetails = _context.QuotationDetails.Where(a => a.QuotationId.Equals(quotationid)).ToList();
            int i = 1;
            foreach (var item in quotationDetails)
            {
                item.SequenceNumber = i;
                i++;
                _context.Attach(item);
                _context.Entry(item).Property("SequenceNumber").IsModified = true;
            }
            _context.SaveChanges();
        }

        private async Task reCalculateSpecialChargesAsync(string quotationid)
        {
            var quotationToUpdate = _context.Quotations.Where(a => a.QuotationId.Equals(quotationid)).Include(a => a.QuotationSpecialCharges).ThenInclude(a => a.SpecialCharges).First();
            var amountsum = Convert.ToSingle(Math.Floor(_context.QuotationDetails.Where(a => a.QuotationId.Equals(quotationid)).Sum(a => a.ProductAmount)));
            var updateflag = 0;
            foreach (var item in quotationToUpdate.QuotationSpecialCharges)
            {
                if (!item.DefaultCalculationOverride && item.SpecialCharges.SpecialChargeType == Enums.SpecialChargeType.PercentageBasedCharge)
                {
                    var specialCharge = await _context.SpecialCharges.FindAsync(item.SpecialChargesId);
                    if (specialCharge.SpecialChargeFixedAmount > 0)
                    {
                        var SCPercAmount = Convert.ToSingle((specialCharge.SpecialChargePercentage * amountsum) / 100);
                        if (SCPercAmount > specialCharge.SpecialChargeFixedAmount)
                        {
                            updateflag = 1;
                            item.SpecialChargeAmount = Convert.ToSingle(Math.Floor(item.SpecialCharges.SpecialChargeFixedAmount));
                            _context.Attach(item);
                            _context.Entry(item).Property("SpecialChargeAmount").IsModified = true;
                        }
                        else
                        {
                            updateflag = 1;
                            item.SpecialChargeAmount = Convert.ToSingle(Math.Floor(SCPercAmount));
                            _context.Attach(item);
                            _context.Entry(item).Property("SpecialChargeAmount").IsModified = true;
                        }
                    }
                    else
                    {
                        updateflag = 1;
                        float SCPercAmount = Convert.ToSingle((specialCharge.SpecialChargePercentage * amountsum) / 100);
                        item.SpecialChargeAmount = Convert.ToSingle(Math.Floor(SCPercAmount));
                        _context.Attach(item);
                        _context.Entry(item).Property("SpecialChargeAmount").IsModified = true;

                    }
                }
            }
            if (updateflag == 1)
            {
                _context.SaveChanges();
            }

            quotationToUpdate.QuotationGrandTotalAmount = (_context.QuotationSpecialCharges.Where(q => q.QuotationId.Equals(quotationid)).Sum(a => a.SpecialChargeAmount)) + amountsum;
            quotationToUpdate.QuotationAmount = amountsum;
            var user = await GetCurrentUserAsync();
            quotationToUpdate.ModifiedByUserId = user.Id;
            quotationToUpdate.QuotationModificationDate = DateTime.Now;
            _context.Attach(quotationToUpdate);
            _context.Entry(quotationToUpdate).Property("QuotationAmount").IsModified = true;
            _context.Entry(quotationToUpdate).Property("QuotationGrandTotalAmount").IsModified = true;
            _context.Entry(quotationToUpdate).Property("ModifiedByUserId").IsModified = true;
            _context.Entry(quotationToUpdate).Property("QuotationModificationDate").IsModified = true;
            _context.SaveChanges();
        }
    }
}
