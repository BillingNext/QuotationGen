using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BillingNextQuotation
{
    public class AcceptedModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public AcceptedModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult OnGet()
        {
            CompanyName = _context.Companies.Select(a => a.CompanyName).FirstOrDefault();
            return Page();
        }

        public string CompanyName { get; set; }
    }
}