using BillingNextQuotation.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CompanyId { get; set; }

        [Required(ErrorMessage = "Please Specify Company Name"), StringLength(100)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CompanyCreationDate { get; set; }

        public byte[] CompanyLogoImg { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [Display(Name = "Company Logo")]
        public IFormFile CompLogo { get; set; }


        [Display(Name = "GST Identification Number")]
        [Required(ErrorMessage = "Please Specify GST Identification Number")]
        [RegularExpression(@"\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}", ErrorMessage = "Not A Valid GST Identification Number")]
        public string GSTIN { get; set; }

        [Required(ErrorMessage = "Please Specify Bank Name... It will be printed on bill"), StringLength(100)]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Please Specify Account Type... It will be printed on bill")]
        [Display(Name = "Bank Account Type")]
        public BankAccountType? BankAccountType { get; set; }

        [RegularExpression(@"^\d{9,18}$", ErrorMessage = "Not A Valid Bank Account Number")]
        [Required(ErrorMessage = "Please Specify Account Number... It will be printed on bill")]
        [Display(Name = "Bank Account Number")]
        public string AccountNumber { get; set; }

        [RegularExpression(@"^[A-Za-z]{4}[a-zA-Z0-9]{7}$", ErrorMessage = "Not A Valid IFSC Code")]
        [Display(Name = "IFSC Code")]
        [Required(ErrorMessage = "Please Specify IFSC Code... It will be printed on bill")]
        public string IFSCcode { get; set; }

        [RegularExpression(@"^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$", ErrorMessage = "Not A Valid PAN")]
        [Display(Name = "Company's PAN")]
        [Required(ErrorMessage = "Please Specify PAN.")]
        public string PAN { get; set; }

        [Display(Name = "Company's Owner")]
        public string CompanyOwner { get; set; }

        [EmailAddress]
        [Display(Name ="Company Email")]
        [Required(ErrorMessage ="Company Email is requird")]
        public string CompanyEmail { get; set; }

        [Phone]
        [Display(Name = "Company Phone Number")]
        [Required(ErrorMessage = "Company Phone Number is requird")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Enter 10 digit phone number")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid Phone Number format.")]
        public string CompanyPhoneNumber { get; set; }

        [Required(ErrorMessage = "Please Specify Company Address"), StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Company Address")]
        public string CompanyAddress { get; set; }

        public ICollection<Products> Products { get; set; }

        public ICollection<Quotation> Quotations { get; set; }

        public ICollection<QuotationNote> QuotationNotes { get; set; }

        public ICollection<SpecialCharges> SpecialCharges { get; set; }
    }
}
