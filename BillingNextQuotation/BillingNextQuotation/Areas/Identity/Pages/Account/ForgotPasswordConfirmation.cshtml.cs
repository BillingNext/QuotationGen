using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BillingNextQuotation.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "SuperAdmin,Developer,Assistant")]
    public class ForgotPasswordConfirmation : PageModel
    {
        public void OnGet()
        {
        }
    }
}
