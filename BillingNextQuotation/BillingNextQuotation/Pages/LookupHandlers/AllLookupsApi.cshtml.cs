using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingNextQuotation.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NonFactors.Mvc.Lookup;

namespace BillingNextQuotation
{
    public class AllLookupsApiModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly UserManager<QuotationGenUser> _userManager;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public AllLookupsApiModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<QuotationGenUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public IActionResult OnGetAllProducts(LookupFilter filter)
        {
            Lookups.ProductsLookup lookup = new Lookups.ProductsLookup(_context) { Filter = filter };
            return new JsonResult(lookup.GetData());
        }

    }
}