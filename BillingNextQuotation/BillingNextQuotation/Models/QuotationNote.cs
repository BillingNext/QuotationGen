using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Models
{
    public class QuotationNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string QuotationNoteId { get; set; }

        [Required(ErrorMessage ="Name of Note can not be empty")]
        [Display(Name ="Identification Name for Note")]
        public string NoteName { get; set; }
        
        [Display(Name ="Quotation Note")]
        [DataType(DataType.Html)]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Quotation Note can not be null.")]
        public string Note { get; set; }

        public bool IsNoteDefault { get; set; }

        [Required]
        public DateTime NoteCreationDate { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public ICollection<Quotation> Quotations { get; set; }
    }
}
