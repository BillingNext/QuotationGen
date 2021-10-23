using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Enums
{
    public enum Roles
    {
        [Display(Name = "Super Admin")]
        SuperAdmin=0,

        [Display(Name = "Assistant")]
        Assistant =1,

        [Display(Name = "Dealers")]
        Dealers =2
    }
}
