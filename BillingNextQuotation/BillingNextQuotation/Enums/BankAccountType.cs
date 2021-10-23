using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Enums
{
    public enum BankAccountType
    {
        [Display(Name = "Current Account")]
        CurrentAcc=0,

        [Display(Name = "Saving Account")]
        SavingAcc=1
    }
}
