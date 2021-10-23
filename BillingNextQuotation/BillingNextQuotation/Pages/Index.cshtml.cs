using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingNextQuotation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BillingNextQuotation.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context, UserManager<QuotationGenUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetCurrentUserAsync();
            var role = await _userManager.GetRolesAsync(user);
            if (role.First().Equals("Dealers"))
            {
                return RedirectToPage("/Dashboard/GenUser/Index");
            }
            else
            {
                return RedirectToPage("/Dashboard/Admin/Index");
            }
        }
    }
}
