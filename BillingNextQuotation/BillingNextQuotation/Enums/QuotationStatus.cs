using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Enums
{
    public enum QuotationStatus
    {
        [Display(Name ="Quotation Created")]
        Created=0,

        [Display(Name = "Quotation in Negotiation")]
        Negotiation =1,

        [Display(Name = "Quotation Finalized")]
        Finalized =2,

        [Display(Name = "Quotation Discarded")]
        Discarded = 3,

        [Display(Name = "Quotation Accepted")]
        Accepted = 4,

        [Display(Name = "Billed")]
        Billed = 5
    }
}
