using BillingNextQuotation.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Areas.Identity.Pages.Account.Manage.Admin
{
    public class QuotationUser
    {

        [Required]
        [Display(Name="User Id")]
        public string UserId { get; set; }

        [Required(ErrorMessage ="Name is required")]
        [Display(Name="Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Role Id")]
        public string RoleId { get; set; }


        [Required(ErrorMessage = "Role Name is required")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Profile Picture is required")]
        public byte[] ProfilePicture { get; set; }

        [Required(ErrorMessage = "Please Specify Address"), StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string PhysicalAddress { get; set; }

        [Required(ErrorMessage = "Customer Category can not be empty")]
        [Display(Name = "Customer Category")]
        public CustomerCategory? CustomerCatagory { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePic { get; set; }

        [Display(Name = "Phone Number Confirmed")]
        public bool PhoneNumberConfirmed { get; set; }
    }
}
