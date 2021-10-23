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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BillingNextQuotation.Quotation.Admin
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
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Quotation Quotation { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Quotation = await _context.Quotations
                .Include(q => q.Company)
                .Include(q => q.QuotationGenUser)
                .Include(q => q.QuotationNote)
                .Include(q => q.QuotationDetails)
                .Include(q => q.QuotationSpecialCharges).ThenInclude(q => q.SpecialCharges).FirstOrDefaultAsync(m => m.QuotationId == id);

            if (Quotation == null)
            {
                return NotFound();
            }
            //ViewData["CompanyId"] =_context.Companies.Select(a=>a.CompanyId).First();
            ViewData["Customers"] = new SelectList(_context.Users.Where(a => a.CustomerCatagory != Enums.CustomerCategory.NotApplicable), "Id", "Name");
            ViewData["QuotationNote"] = new SelectList(_context.QuotationNotes, "QuotationNoteId", "NoteName");
            return Page();
        }

        public async Task<IActionResult> OnPutQuotationUpdateAsync(string id, [FromBody] Models.Quotation obj)
        {
            try { 
                var user= await GetCurrentUserAsync();
                var quotationToUpdate = await _context.Quotations.FindAsync(id);
                quotationToUpdate.QuotationModificationDate = DateTime.Now;
                quotationToUpdate.ModifiedByUserId = user.Id;
                quotationToUpdate.QuotationTo = obj.QuotationTo;
                quotationToUpdate.QuotationStatus = obj.QuotationStatus;
                quotationToUpdate.QuotationCreationDate = obj.QuotationCreationDate;
                quotationToUpdate.QuotationNoteId = obj.QuotationNoteId;
                quotationToUpdate.QuotationForId = obj.QuotationForId;

                _context.Attach(quotationToUpdate);
                _context.Entry(quotationToUpdate).Property("QuotationModificationDate").IsModified = true;
                _context.Entry(quotationToUpdate).Property("ModifiedByUserId").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationTo").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationStatus").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationCreationDate").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationNoteId").IsModified = true;
                _context.Entry(quotationToUpdate).Property("QuotationForId").IsModified = true;
                _context.SaveChanges();
                return new JsonResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuotationExists(Quotation.QuotationId))
                {
                    return new JsonResult("Quotation not found");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                return new JsonResult(e.ToString());
            }
        }

        public JsonResult OnGetCustomersAddress(string id)
        {
            return new JsonResult(_context.Users.Where(a => a.Id.Equals(id)).Select(a => a.PhysicalAddress));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Quotation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuotationExists(Quotation.QuotationId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool QuotationExists(string id)
        {
            return _context.Quotations.Any(e => e.QuotationId == id);
        }

        private async Task InitDetailsAsync()
        {
            Quotation = await _context.Quotations
                .Include(q => q.Company)
                .Include(q => q.QuotationGenUser)
                .Include(q => q.QuotationNote)
                .FirstOrDefaultAsync(m => m.QuotationId == Quotation.QuotationId);

            ViewData["Customers"] = new SelectList(_context.Users.Where(a => a.CustomerCatagory != Enums.CustomerCategory.NotApplicable), "Id", "Name");
            ViewData["QuotationNote"] = new SelectList(_context.QuotationNotes, "QuotationNoteId", "NoteName");
        }
    }
}