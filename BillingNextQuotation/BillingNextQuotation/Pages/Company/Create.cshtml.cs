using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using BillingNextQuotation.Enums;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace BillingNextQuotation.Company
{
    [Authorize(Roles = "SuperAdmin,Developer")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            if(_context.Companies.Any())
            {
                return NotFound("Your current plan only supports making 1 company. Contact support at +91 8732992181 for upgrading plan.");
            }
            return Page();
        }

        [BindProperty]
        public Models.Company Company { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Company.CompLogo != null)
            {
                if (Company.CompLogo.Length > 0)

                //Convert Image to byte and save to database

                {
                    byte[] p1 = null;
                    using (var fs1 = Company.CompLogo.OpenReadStream())
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                    Company.CompanyLogoImg = p1;
                }
            }
            Company.CompanyCreationDate = DateTime.Now;
            _context.Companies.Add(Company);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
