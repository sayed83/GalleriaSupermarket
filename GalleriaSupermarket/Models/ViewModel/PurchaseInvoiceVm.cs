using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class PurchaseInvoiceVm
    {
        public int PurchaseInvoiceID { get; set; }
        public string PartNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ShppingCost { get; set; }
        public decimal DueAmount { get; set; }
        public decimal NetAmount { get; set; }
        public bool IsPaid { get; set; }
        public string InvoiceDate { get; set; }

        public int SupplierID { get; set; }     
        public string CompanyName { get; set; }

        public int OutletID { get; set; }
        public string OutletName { get; set; }
        public string ShortAddress { get; set; }

    }
}