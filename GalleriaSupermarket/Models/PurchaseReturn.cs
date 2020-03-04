using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class PurchaseReturn
    {
        [Key]
        public int PurchaseReturnID { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "Invoice Number")]
        public int PurchaseInvoiceID { get; set; }
        public virtual PurchaseInvoice PurchaseInvoice { get; set; }
        public string Note { get; set; }
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }
    }
}