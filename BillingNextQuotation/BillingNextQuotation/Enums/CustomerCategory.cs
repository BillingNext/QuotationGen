using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Enums
{
    public enum CustomerCategory
    {
        [Display(Name = "Not Applicable")]
        NotApplicable = 0,

        [Display(Name = "Category A")]
        CategoryA = 1,

        [Display(Name ="Category B")]
        CategoryB = 2,

        [Display(Name ="Category C")]
        CategoryC = 3,

        [Display(Name ="Category D")]
        CategoryD = 4
    }
}
