using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalleriaSupermarket.Models
{
    public class SupplierPayment
    {
        [Key]
        public int SupplierPaymentID { get; set; }
        public int PurchaseInvoiceID { get; set; }
        public virtual PurchaseInvoice PurchaseInvoice { get; set; }
        [Display(Name ="Paid Amount"), Required]
        public decimal PaidAmount { get; set; }
        [Display(Name ="Payment Date")]
        public DateTime PaymentDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}