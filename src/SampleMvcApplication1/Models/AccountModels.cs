using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace SampleMvcApplication1.Models
{
    public class LogOnModel
    {
        [Required]
        [Display(Name = "Shop name", Description = "Just the shop name part of shopname.myshopify.com")]
        public string ShopName { get; set; }
    }
}
