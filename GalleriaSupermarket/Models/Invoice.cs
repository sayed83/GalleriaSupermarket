using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }
        [Display(Name = "Invoice No.")]
        public string PartNumber { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Phone")]
        public string ContactNumber { get; set; }
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Discount")]
        public decimal InvoiceDiscount { get; set; }
        [Display(Name = "Order By")]
        public string MadyBy { get; set; }
        public int OutletID { get; set; }
        public virtual Outlet Outlet { get; set; }
        [Display(Name = "Status")]
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime InvoiceDate { get; set; }
        public virtual ICollection<InternalSale> InternalSales { get; set; }
    }
}