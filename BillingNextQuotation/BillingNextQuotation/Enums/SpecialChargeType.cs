using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Enums
{
    public enum SpecialChargeType
    {
        [Display(Name ="Fixed Charge")]
        FixedCharge=0,

        [Display(Name = "Percentage Based Charge")]
        PercentageBasedCharge =1
    }
}
