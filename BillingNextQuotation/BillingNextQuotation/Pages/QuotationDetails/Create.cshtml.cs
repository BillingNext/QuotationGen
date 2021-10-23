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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BillingNextQuotation.QuotationDetails
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
            ViewData["Product"] = new SelectList(_context.Products, "ProductId", "ProductName");
            QuotationId = id;
            Quotation = _context.Quotations.Where(a => a.QuotationId.Equals(id)).FirstOrDefault();
            return Page();
        }

        [BindProperty]
        public Models.QuotationDetails QuotationDetails { get; set; }

        public Models.Quotation Quotation { get; set; }

        public string QuotationId { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (_context.QuotationDetails.Where(a => a.QuotationId.Equals(QuotationDetails.QuotationId)).Any())
                {
                    QuotationDetails.SequenceNumber = (_context.QuotationDetails.Where(a => a.QuotationId.Equals(QuotationDetails.QuotationId)).Max(a => a.SequenceNumber)) + 1;
                }
                else
                {
                    QuotationDetails.SequenceNumber = 1;
                }
                var product = _context.Products.Where(a => a.ProductId.Equals(QuotationDetails.ProductId)).First();
                if(QuotationDetails.SheetSizingOptions==Enums.SheetSizingOptions.FullSheet)
                {
                    QuotationDetails.ProductDimensionX = product.ProductDimensionX;
                    QuotationDetails.ProductDimensionY = product.ProductDimensionY;
                }
                QuotationDetails.ProductName = product.ProductName;
                if (product.ProductDimensionX < QuotationDetails.ProductDimensionX || product.ProductDimensionY < QuotationDetails.ProductDimensionY)
                {
                    ViewData["Product"] = new SelectList(_context.Products, "ProductId", "ProductName");
                    QuotationId = QuotationDetails.QuotationId;
                    Quotation = _context.Quotations.Where(a => a.QuotationId.Equals(QuotationDetails.QuotationId)).FirstOrDefault();
                    ModelState.AddModelError(string.Empty, "Product Dimension is more than maximum product area.");
                    return Page();
                }
                if (QuotationDetails.SheetSizingOptions == Enums.SheetSizingOptions.Sizes)
                {
                    QuotationDetails.ProductDimensionX = Convert.ToSingle(QuotationDetails.ProductDimensionX % 6 == 0 ? QuotationDetails.ProductDimensionX : Math.Ceiling(QuotationDetails.ProductDimensionX / 6) * 6);
                    QuotationDetails.ProductDimensionY = Convert.ToSingle(QuotationDetails.ProductDimensionY % 6 == 0 ? QuotationDetails.ProductDimensionY : Math.Ceiling(QuotationDetails.ProductDimensionY / 6) * 6);
                }
                else
                {
                    QuotationDetails.ProductDimensionX = product.ProductDimensionX;
                    QuotationDetails.ProductDimensionY = product.ProductDimensionY;
                }
                
                QuotationDetails.TotalArea = QuotationDetails.ProductDimensionX * QuotationDetails.ProductDimensionY;

                var squarefoot = QuotationDetails.TotalArea / 144;
                QuotationDetails.ProductAmount = QuotationDetails.ProductQuantity * (squarefoot * QuotationDetails.ProductRate);
                QuotationDetails.CompanyId = _context.Companies.Select(a => a.CompanyId).FirstOrDefault();
                _context.QuotationDetails.Add(QuotationDetails);
                await _context.SaveChangesAsync();
                await reCalculateSpecialChargesAsync(QuotationDetails.QuotationId);
                return Redirect($"/Quotation/Admin/Edit/{QuotationDetails.QuotationId}");
            }
            catch(Exception e)
            {
                ViewData["Product"] = new SelectList(_context.Products, "ProductId", "ProductName");
                QuotationId = QuotationDetails.QuotationId;
                Quotation = _context.Quotations.Where(a => a.QuotationId.Equals(QuotationDetails.QuotationId)).FirstOrDefault();
                ModelState.AddModelError(string.Empty, "Some errors occured adding product entry in quotation.");
                return Page();
            }
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
                            item.SpecialChargeAmount =Convert.ToSingle(Math.Floor(SCPercAmount));
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
            if(updateflag==1)
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
