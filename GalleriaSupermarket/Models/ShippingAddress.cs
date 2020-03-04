using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class ShippingAddress
    {
        [Key]
        public int ShippingAddressID { get; set; }
        public int CityID { get; set; }
        public virtual City City { get; set; }
        public string Address { get; set; }
        [Display(Name = "Phone")]
        public string ContactNumber { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public virtual ICollection<OnlineInvoice> OnlineInvoices { get; set; }
    }
}