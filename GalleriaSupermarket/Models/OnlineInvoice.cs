using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class OnlineInvoice
    {
        [Key]
        public int OnlineInvoiceID { get; set; }
        [Display(Name = "Invoice No.")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        [Display(Name = "Payer Number"), Required]
        public string PayerNumber { get; set; }
        [Display(Name = "TransactionID"), Required]
        public string TransNumber { get; set; }
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        [Display(Name = "Customer Name")]
        public string UserName { get; set; }
        [Display(Name = "Address")]
        public int ShippingAddressID { get; set; }
        [Display(Name = "Outlet")]
        public int OutletID { get; set; }
        public virtual Outlet Outlet { get; set; }
        public virtual ShippingAddress ShippingAddress { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime OrderDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<OnlineSale> OnlineSales { get; set; }
        public virtual ICollection<SalesReturn> SalesReturns { get; set; }
    }
}