using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BillingNextQuotation.Data;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BillingNextQuotation.Quotation
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        public IndexModel(BillingNextQuotation.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
          
        }
    }
    public class IndexGridModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public IndexGridModel(ApplicationDbContext context, UserManager<QuotationGenUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IQueryable<Models.Quotation> Quotations { get; set; }

        public async Task OnGetAsync()
        {
            var user = await GetCurrentUserAsync();
            Quotations = _context.Quotations.Where(a => a.QuotationForId.Equals(user.Id) && !(a.QuotationStatus.Equals(Enums.QuotationStatus.Discarded))).OrderByDescending(a=>a.QuotationNumber).AsQueryable();
        }
    }
}
