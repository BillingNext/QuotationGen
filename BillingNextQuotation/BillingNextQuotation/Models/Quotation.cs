using BillingNextQuotation.Data;
using BillingNextQuotation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Models
{
    public class Quotation
    {
        [Key]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string QuotationId { get; set; }

        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuotationNumber { get; set; }

        [Display(Name ="Quotation Amount")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage ="Quotation Amount can not be empty")]
        public float QuotationAmount{ get; set; }

        [Display(Name = "Quotation Grand Total Amount")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Quotation Grand Total Amount can not be empty")]
        public float QuotationGrandTotalAmount { get; set; }

        [Required(ErrorMessage ="Quotation To is empty")]
        [Display(Name ="Quotation To")]
        public string QuotationTo { get; set; }

        [Display(Name ="Quotation Status")]
        [Required(ErrorMessage ="Quotation Status in null")]
        public QuotationStatus QuotationStatus { get; set; }

        [ScaffoldColumn(false)]
        [Required]
        public DateTime QuotationCreationDate { get; set; }

        [ScaffoldColumn(false)]
        [Required]
        public DateTime ActualQuotationCreationDate { get; set; }

        [ScaffoldColumn(false)]
        [Required]
        public DateTime? QuotationModificationDate { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }

        public string? ModifiedByUserId { get; set; }

        [StringLength(6,MinimumLength =6, ErrorMessage = "Enter a 6 digit code")]
        [Required(ErrorMessage ="Secret Code is required")]
        public string SecretCode { get; set; }

        [Required(AllowEmptyStrings =true)]
        public string QuotationNoteId { get; set; }

        [ForeignKey("QuotationNoteId")]
        public QuotationNote QuotationNote { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [Required]
        public string QuotationForId { get; set; }

        [ForeignKey("QuotationForId")]
        public QuotationGenUser QuotationGenUser { get; set; }

        public ICollection<QuotationSpecialCharges> QuotationSpecialCharges { get; set; }

        public ICollection<QuotationDetails> QuotationDetails { get; set; }
    }
}
