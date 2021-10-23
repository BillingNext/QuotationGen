using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Enums
{
    public enum SheetSizingOptions
    {
        [Display(Name ="Sizes")]
        Sizes=0,

        [Display(Name = "Full Sheet")]
        FullSheet = 1
    }
}
