using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingNextQuotation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NonFactors.Mvc.Grid;

namespace BillingNextQuotation.Quotation.Admin
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }
    }

    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class IndexGridModel : PageModel
    {
        private readonly ApplicationDbContext _context;
       
        public IndexGridModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<Models.Quotation> Quotations { get; set; }

        public async Task OnGetAsync()
        {
            Quotations = _context.Quotations.Include(q=>q.QuotationGenUser).OrderByDescending(a=>a.QuotationNumber).AsQueryable();
        }
    }

}