using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Enums
{
    public enum SheetMeasurementOptions
    {
        [Display(Name ="Inches")]
        Inch=0,

        [Display(Name = "Feet-Inch")]
        FeetInch = 1,

        [Display(Name = "Centimeters")]
        CM=2
    }
}
