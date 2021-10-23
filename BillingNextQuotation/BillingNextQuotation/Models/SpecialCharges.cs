using BillingNextQuotation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Models
{
    public class SpecialCharges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SpecialChargesId { get; set; }

        [Display(Name ="Special Charge Name")]
        [Required(ErrorMessage ="Special Charge Name can not be null")]
        public string SpecialChargeName { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required(ErrorMessage ="Special Charge Type is empty.")]
        [Display(Name = "Special Charge Type")]
        public SpecialChargeType SpecialChargeType { get; set; }

        [Display(Name = "Special Charge Fixed Amount or Max Amount to be charged(in case of Percentage based charge).")]
        [Required(ErrorMessage = "Special Charges amount can not be null")]
        [DataType(DataType.Currency)]
        public float SpecialChargeFixedAmount { get; set; }

        [Display(Name = "Special Charge as Percentage of Total Amount")]
        public float? SpecialChargePercentage { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public ICollection<QuotationSpecialCharges> QuotationSpecialCharges { get; set; }
    }
}
