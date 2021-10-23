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
using BillingNextQuotation.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.Quotation
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class CreateModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly ILogger<CreateModel> _logger;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public CreateModel(BillingNextQuotation.Data.ApplicationDbContext context, UserManager<QuotationGenUser> userManager, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            QuotationUser = await GetCurrentUserAsync();
            Company = _context.Companies.FirstOrDefault();
            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return Page();
        }

        public Models.Company Company { get; set; }

        public Models.Products Products { get; set; }

        public QuotationGenUser QuotationUser { get; set; }

        [BindProperty]
        public Models.QuotationDetails QuotationDetails { get; set; }

        public class ProductDetails
        {
            public float ProductDimensionX { get; set; }

            public float ProductDimensionY { get; set; }

            public float Rate { get; set; }
        }
     
        public JsonResult OnGetProductSizes(string id)
        {
            return new JsonResult(_context.Products.Where(a => a.ProductId.Equals(id)).Select(a => new ProductDetails{ProductDimensionX= a.ProductDimensionX, ProductDimensionY= a.ProductDimensionY }));  
        }

        public async Task<JsonResult> OnPostCreateQuotationAsync([FromBody] Models.Quotation obj)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                obj.CreatedByUserId = user.Id;
                
                obj.QuotationForId = user.Id;
                obj.QuotationTo = _context.Users.Where(a => a.Id.Equals(user.Id)).Select(a => a.UserName).First();
                obj.QuotationCreationDate = DateTime.Now;
                obj.QuotationModificationDate = DateTime.Now;
                obj.ActualQuotationCreationDate = DateTime.Now;
                obj.QuotationNoteId = _context.QuotationNotes.Where(a => a.IsNoteDefault == true).Select(a => a.QuotationNoteId).First();
                obj.QuotationAmount = 0;
                Random generator = new Random();
                obj.SecretCode= generator.Next(0, 999999).ToString("D6");
                _context.Quotations.Add(obj);
                _context.SaveChanges();
                _logger.LogInformation("Saved Quotation");
                return new JsonResult(obj.QuotationId);
            }
            catch(Exception e)
            {
                _logger.LogError("Exception while saving Quoation: " + e.ToString());
                return new JsonResult("Error occured making Quotation");
            }
        }


        public JsonResult OnPostCreateQuotationDetails([FromBody] Models.QuotationDetails obj)
        {
            try
            {
                var product = _context.Products.Where(a => a.ProductId.Equals(obj.ProductId)).First();

                if (product.ProductDimensionX < obj.ProductDimensionX || product.ProductDimensionY < obj.ProductDimensionY)
                {
                    return new JsonResult("Product Dimension is more than maximum product area.");
                }

                if (obj.SheetSizingOptions == Enums.SheetSizingOptions.Sizes)
                {
                    obj.ProductDimensionX = Convert.ToSingle(obj.ProductDimensionX % 6 == 0 ? obj.ProductDimensionX : Math.Ceiling(obj.ProductDimensionX / 6) * 6);
                    obj.ProductDimensionY = Convert.ToSingle(obj.ProductDimensionY % 6 == 0 ? obj.ProductDimensionY : Math.Ceiling(obj.ProductDimensionY / 6) * 6);
                }
                else
                {
                    obj.ProductDimensionX = product.ProductDimensionX;
                    obj.ProductDimensionY = product.ProductDimensionY;
                }
                obj.TotalArea = obj.ProductDimensionX * obj.ProductDimensionY;


                if (obj.SheetSizingOptions == Enums.SheetSizingOptions.Sizes)
                {
                    obj.ProductRate = product.ProductPieceCutPrice;
                }
                else
                {
                    obj.ProductRate = product.ProductFullSheetPrice;
                }

                var squarefoot = obj.TotalArea / 144;
                obj.ProductAmount = obj.ProductQuantity * (squarefoot * obj.ProductRate);
                _context.QuotationDetails.Add(obj);
                _context.SaveChanges();

                return new JsonResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception in saving Quotation Detail:" + e.ToString());
                return new JsonResult(false);
            }
        }

        public async Task<JsonResult> OnPostUpdateQuotationAmountAsync(string id)
        {
            try
            {
                var quotationToUpdate = await _context.Quotations.FindAsync(id);
                var amountsum = Convert.ToSingle(Math.Floor(_context.QuotationDetails.Where(a => a.QuotationId.Equals(id)).Sum(a => a.ProductAmount)));
                var specialchargeslist = _context.SpecialCharges.Where(a => a.IsDefault == true).ToList();
                var companyid = _context.Companies.Select(a=>a.CompanyId).FirstOrDefault();
                foreach (var item in specialchargeslist)
                {
                    if(item.SpecialChargeType==Enums.SpecialChargeType.FixedCharge)
                    {
                        _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.SpecialChargesId, SpecialChargeAmount =Convert.ToSingle(Math.Floor(item.SpecialChargeFixedAmount)), CompanyId=companyid , DefaultCalculationOverride=false});
                    }
                    else
                    {
                        if(item.SpecialChargeFixedAmount>0)
                        {
                            var SCPercAmount =Convert.ToSingle((item.SpecialChargePercentage * amountsum) / 100);
                            if (SCPercAmount > item.SpecialChargeFixedAmount)
                            {
                                _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.SpecialChargesId, SpecialChargeAmount = Convert.ToSingle(Math.Floor(item.SpecialChargeFixedAmount)), CompanyId = companyid, DefaultCalculationOverride = false });
                            }
                            else
                            {
                                _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.SpecialChargesId, SpecialChargeAmount = Convert.ToSingle(Math.Floor(SCPercAmount)), CompanyId = companyid, DefaultCalculationOverride = false });
                            }
                        }
                        else
                        {
                            float SCPercAmount =Convert.ToSingle((item.SpecialChargePercentage * amountsum) / 100);
                            _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.SpecialChargesId, SpecialChargeAmount = Convert.ToSingle(Math.Floor(SCPercAmount)), CompanyId = companyid, DefaultCalculationOverride = false });
                        }
                    }
                }
                _context.SaveChanges();
                quotationToUpdate.QuotationGrandTotalAmount = _context.QuotationSpecialCharges.Where(q => q.QuotationId.Equals(id)).Sum(a => a.SpecialChargeAmount)+ amountsum;
                quotationToUpdate.QuotationAmount = amountsum;
                _context.Attach(quotationToUpdate);
                _context.Entry(quotationToUpdate).Property("QuotationAmount").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationGrandTotalAmount").IsModified = true;
                _context.SaveChanges();
                return new JsonResult(true);

            }
            catch (Exception e)
            {
                _logger.LogError("Error occured updating Quotation Amount: "+e.ToString());
                return new JsonResult(false);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.QuotationDetails.Add(QuotationDetails);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
