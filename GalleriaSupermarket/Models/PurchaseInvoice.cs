using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class PurchaseInvoice
    {
        [Key]
        public int PurchaseInvoiceID { get; set; }
        [Display(Name = "Invoice No.")]
        public string PartNumber { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Paid")]
        public decimal PaidAmount { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Shipping Cost")]
        public decimal ShppingCost { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Due")]
        public decimal DueAmount { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Net Price")]
        public decimal NetAmount { get; set; }
        [Display(Name = "Status")]
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }
        [Display(Name = "Supplier")]
        public int SupplierID { get; set; }
        public virtual Supplier Supplier { get; set; }
        [Display(Name = "Outlet")]
        public int OutletID { get; set; }
        public virtual Outlet Outlet { get; set; }
        public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; }
        public virtual ICollection<SupplierPayment> SupplierPayments { get; set; }

    }
}