using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }
        [Display(Name ="Company Name")]
        [Required]
        public string CompanyName { get; set; }
        [Display(Name ="Address")]
        public string OfficeAddress { get; set; }
        [Display(Name ="Phone"), Required]
        public string ContactNumber { get; set; }
        public string Owner { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
    }
}