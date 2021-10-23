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

namespace BillingNextQuotation.Redirector
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;
        private readonly UserManager<QuotationGenUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        private Task<QuotationGenUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        public IndexModel(BillingNextQuotation.Data.ApplicationDbContext context, UserManager<QuotationGenUser> userManager, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync(string service,string subservice)
        {
            if(service==null || subservice==null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();
            var role =await _userManager.GetRolesAsync(user);
            if(role.FirstOrDefault().Equals("Dealers"))
            {
                return RedirectToPage("/"+service+"/"+subservice);
            }
            else
            {
                return RedirectToPage("/" + service+"/Admin/"+subservice);
            }
        }
    }
}