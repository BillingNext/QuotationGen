using BillingNextQuotation.Enums;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Data
{
    public class QuotationGenUser: IdentityUser
    {
        [Required(ErrorMessage = "Customer Name can not be empty")]
        [Display(Name = "Customer Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Specify Address"), StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string PhysicalAddress { get; set; }

        [Required(ErrorMessage ="Customer Category can not be empty")]
        [Display(Name ="Customer Category")]
        public CustomerCategory? CustomerCatagory { get; set; }

        public byte[] ProfilePicture { get; set; }

        public ICollection<Models.Quotation> Quotations { get; set; }

        public ICollection<Models.Products> Products { get; set; }
    }
}
