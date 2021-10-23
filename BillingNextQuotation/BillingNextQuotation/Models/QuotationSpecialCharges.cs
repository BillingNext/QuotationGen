using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Models
{
    public class QuotationSpecialCharges
    {
        [Required(ErrorMessage = "Please specify a special charge")]
        [Display(Name = "Special Charge Name")]
        public string SpecialChargesId { get; set; }

        [ForeignKey("SpecialChargesId")]
        public SpecialCharges SpecialCharges { get; set; }

        public string QuotationId { get; set; }

        [ForeignKey("QuotationId")]
        public Quotation Quotation { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [Display(Name = "Special Charge Amount")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Special Charge Amount can not be empty")]
        public float SpecialChargeAmount { get; set; }

        [Required]
        [Display(Name = "Override Default Calculation?")]
        public bool DefaultCalculationOverride { get; set; }

    }
}
