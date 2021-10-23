using BillingNextQuotation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Models
{
    public class QuotationDetails
    {
        [Key]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string QuotationDetailsId { get; set; }

        [Display(Name ="Sequence Number",Order =1)]
        public int SequenceNumber { get; set; }

        [Display(Name = "Name of the Product",Order =2)]
        [Required(ErrorMessage ="Particulars is Empty")]
        public string ProductName { get; set; }


        [Required(ErrorMessage = "Sheet Measurement Option can not be null")]
        [Display(Name = "Sheet Measurement Option",Order =3)]
        public SheetMeasurementOptions SheetMeasurementOptions { get; set; }

        [Required(ErrorMessage ="Sheet Sizing Option can not be null")]
        [Display(Name ="Sheet Sizing Option",Order =4)]
        public SheetSizingOptions SheetSizingOptions { get; set; }

        [Display(Name ="Width of Glass (Dimension X)",Order =5)]
        [Required]
        public float ProductDimensionX { get; set; }

        [Display(Name = "Height of Glass (Dimension Y)", Order =6)]
        [Required]
        public float ProductDimensionY { get; set; }

        [Display(Name ="Quantity of Sheets",Order =7)]
        [Required(ErrorMessage ="Product Quantity can not be null")]
        [Range(1,1000)]
        public int ProductQuantity { get; set; }

        [Display(Name ="Rate Charged",Order =8)]
        [Required(ErrorMessage ="Rate of Sheet can not be empty")]
        public float ProductRate { get; set; }

        [Display(Name ="Total Area in Square Foot",Order =9)]
        [Required]
        public float TotalArea { get; set; }

        [Display(Name ="Total Amount of Product",Order =10)]
        [Required(ErrorMessage ="Total Amount of Product can not be null")]
        [DataType(DataType.Currency)]
        public float ProductAmount { get; set; }

        [Required]
        public string QuotationId { get; set; }

        [ForeignKey("QuotationId")]
        public Quotation Quotation { get; set; }

        [Required]
        public string ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Products Products { get; set; }

        [Required]
        public string CompanyId { get; set; }
    }
}
