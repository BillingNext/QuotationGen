using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingNextQuotation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace BillingNextQuotation.Quotation.Admin
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
            _logger = logger;
            _userManager = userManager;
        }

        public class SpecialChargeModel
        {
            public string Id { get; set; }

            public float Charge { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["Customers"] = new SelectList(_context.Users.Where(a => a.CustomerCatagory!=Enums.CustomerCategory.NotApplicable), "Id", "Name");
            Company = _context.Companies.FirstOrDefault();
            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "ProductName");
            ViewData["QuotationNote"] = new SelectList(_context.QuotationNotes.Where(a=>a.IsNoteDefault==false), "QuotationNoteId", "NoteName");
            ViewData["SpecialCharges"] = new SelectList(_context.SpecialCharges.Where(a=>a.IsDefault==false), "SpecialChargesId", "SpecialChargeName");
            SpecialCharges = _context.SpecialCharges.Where(a => a.IsDefault == true).ToList();
            return Page();
        }


        public Models.Company Company { get; set; }

        public Models.Products Products { get; set; }

        public IList<Models.SpecialCharges> SpecialCharges { get; set; }

        [BindProperty]
        public Models.QuotationDetails QuotationDetails { get; set; }

        public class ProductDetails
        {
            public float ProductDimensionX { get; set; }

            public float ProductDimensionY { get; set; }

            public float RateFullSheet { get; set; }

            public float RatePeiceSheet { get; set; }
        }

        public async Task<JsonResult> OnPostSpecialChargesWithAmoutUpdateAsync([FromBody] IEnumerable<SpecialChargeModel> obj , string id)
        {
            try
            {
                obj= obj.GroupBy(a => a.Id).Select(g => g.First());
                var quotationToUpdate = await _context.Quotations.FindAsync(id);
                var amountsum = Convert.ToSingle(Math.Floor(_context.QuotationDetails.Where(a => a.QuotationId.Equals(id)).Sum(a => a.ProductAmount)));
                var companyid = _context.Companies.Select(a => a.CompanyId).FirstOrDefault();
                foreach (var item in obj)
                {
                    var specialCharge = await _context.SpecialCharges.FindAsync(item.Id);
                    if (item.Charge == -1)
                    {
                        if (specialCharge.SpecialChargeType == Enums.SpecialChargeType.FixedCharge)
                        {
                            _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.Id, SpecialChargeAmount =Convert.ToSingle(Math.Floor(specialCharge.SpecialChargeFixedAmount)), CompanyId = companyid, DefaultCalculationOverride=false });
                        }
                        else
                        {
                            if (specialCharge.SpecialChargeFixedAmount > 0)
                            {
                                float SCPercAmount =Convert.ToSingle(Math.Floor(Convert.ToSingle((specialCharge.SpecialChargePercentage * amountsum) / 100)));
                                if (SCPercAmount > specialCharge.SpecialChargeFixedAmount)
                                {
                                    _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = specialCharge.SpecialChargesId, SpecialChargeAmount = specialCharge.SpecialChargeFixedAmount, CompanyId = companyid, DefaultCalculationOverride=false });
                                }
                                else
                                {
                                    _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = specialCharge.SpecialChargesId, SpecialChargeAmount = SCPercAmount, CompanyId = companyid, DefaultCalculationOverride=false });
                                }
                            }
                            else
                            {
                                float SCPercAmount = Convert.ToSingle(Math.Floor(Convert.ToSingle((specialCharge.SpecialChargePercentage * amountsum) / 100)));
                                _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = specialCharge.SpecialChargesId, SpecialChargeAmount = SCPercAmount, CompanyId = companyid, DefaultCalculationOverride=false });
                            }
                        }
                    }
                    else
                    {
                        _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.Id, SpecialChargeAmount =Convert.ToSingle(Math.Floor(item.Charge)), CompanyId = companyid, DefaultCalculationOverride=true });
                    }
                }
                _context.SaveChanges();
                quotationToUpdate.QuotationGrandTotalAmount = _context.QuotationSpecialCharges.Where(q => q.QuotationId.Equals(id)).Sum(a => a.SpecialChargeAmount) + amountsum;
                quotationToUpdate.QuotationAmount = amountsum;
                _context.Attach(quotationToUpdate);
                _context.Entry(quotationToUpdate).Property("QuotationAmount").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationGrandTotalAmount").IsModified = true;
                _context.SaveChanges();
                return new JsonResult(true);

            }
            catch (Exception e)
            {
                _logger.LogError("Error occured updating Quotation Amount: " + e.ToString());
                return new JsonResult(false);
            }
        }

        public JsonResult OnGetProductSizes(string id)
        {
            return new JsonResult(_context.Products.Where(a => a.ProductId.Equals(id)).Select(a => new ProductDetails { ProductDimensionX = a.ProductDimensionX, ProductDimensionY = a.ProductDimensionY,RateFullSheet=a.ProductFullSheetPrice, RatePeiceSheet=a.ProductPieceCutPrice }));
        }

        public async Task<JsonResult> OnPostCreateQuotationAsync([FromBody] Models.Quotation obj)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                obj.CreatedByUserId = user.Id;
                obj.ActualQuotationCreationDate = DateTime.Now;
                obj.QuotationModificationDate = DateTime.Now;
                obj.QuotationAmount = 0;
                if(String.IsNullOrEmpty(obj.QuotationNoteId))
                {
                    obj.QuotationNoteId = _context.QuotationNotes.Where(a=>a.IsNoteDefault==true).Select(a=>a.QuotationNoteId).FirstOrDefault();
                }
                Random generator = new Random();
                obj.SecretCode = generator.Next(0, 999999).ToString("D6");
                _context.Quotations.Add(obj);
                _context.SaveChanges();
                _logger.LogInformation("Saved Quotation");
                return new JsonResult(obj.QuotationId);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception while saving Quoation: " + e.ToString());
                return new JsonResult("Error occured making Quotation");
            }
        }

        public async Task<JsonResult> OnPostCreateQuotationDetailsAsync([FromBody] Models.QuotationDetails obj)
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
                var companyid = _context.Companies.Select(a => a.CompanyId).FirstOrDefault();
                foreach (var item in specialchargeslist)
                {
                    if (item.SpecialChargeType == Enums.SpecialChargeType.FixedCharge)
                    {
                        _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.SpecialChargesId, SpecialChargeAmount = item.SpecialChargeFixedAmount, CompanyId = companyid });
                    }
                    else
                    {
                        if (item.SpecialChargeFixedAmount > 0)
                        {
                            var SCPercAmount = Convert.ToSingle((item.SpecialChargePercentage * amountsum) / 100);
                            if (SCPercAmount > item.SpecialChargeFixedAmount)
                            {
                                _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.SpecialChargesId, SpecialChargeAmount = item.SpecialChargeFixedAmount, CompanyId = companyid });
                            }
                            else
                            {
                                _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.SpecialChargesId, SpecialChargeAmount = SCPercAmount, CompanyId = companyid });
                            }
                        }
                        else
                        {
                            float SCPercAmount = Convert.ToSingle((item.SpecialChargePercentage * amountsum) / 100);
                            _context.QuotationSpecialCharges.Add(new Models.QuotationSpecialCharges { QuotationId = id, SpecialChargesId = item.SpecialChargesId, SpecialChargeAmount = SCPercAmount, CompanyId = companyid });
                        }
                    }
                }
                _context.SaveChanges();
                quotationToUpdate.QuotationGrandTotalAmount = _context.QuotationSpecialCharges.Where(q => q.QuotationId.Equals(id)).Sum(a => a.SpecialChargeAmount) + amountsum;
                quotationToUpdate.QuotationAmount = amountsum;
                _context.Attach(quotationToUpdate);
                _context.Entry(quotationToUpdate).Property("QuotationAmount").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationGrandTotalAmount").IsModified = true;
                _context.SaveChanges();
                return new JsonResult(true);

            }
            catch (Exception e)
            {
                _logger.LogError("Error occured updating Quotation Amount: " + e.ToString());
                return new JsonResult(false);
            }
        }
        public JsonResult OnGetCustomersAddress(string id)
        {
            return new JsonResult(_context.Users.Where(a => a.Id.Equals(id)).Select(a => a.PhysicalAddress));
        }

        public JsonResult OnGetSpecialChargeAmount(string id)
        {
            return new JsonResult(_context.SpecialCharges.Where(a => a.SpecialChargesId.Equals(id)).Select(a => a.SpecialChargeFixedAmount));
        }
    }
}