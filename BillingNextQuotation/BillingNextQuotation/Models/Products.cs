using BillingNextQuotation.Data;
using NonFactors.Mvc.Lookup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Models
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ProductId { get; set; }

        [LookupColumn]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Product Name can not be empty.")]
        [Display(Name ="Product Name")]
        public string ProductName { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name ="Product Full Sheet Price")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Product Full Sheet Price can not be empty.")]
        public float ProductFullSheetPrice { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Product Piece Cut Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Product Piece Cut Price can not be empty.")]
        public float ProductPieceCutPrice { get; set; }

        [Display(Name = "Full Sheet Width of Glass (in Inches)")]
        [Required]
        public float ProductDimensionX { get; set; }

        [Display(Name = "Full Sheet Height of Glass (in Inches)")]
        [Required]
        public float ProductDimensionY { get; set; }

        [ScaffoldColumn(false)]
        [Required]
        public DateTime ProductCreationDate { get; set; }

        [ScaffoldColumn(false)]
        [Required]
        public DateTime ProductModificationDate { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }

        public string? ModifiedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public QuotationGenUser QuotationGenUser { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public ICollection<QuotationDetails> QuotationDetails { get; set; }
    }
}
