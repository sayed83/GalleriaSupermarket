using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseID { get; set; }
        public int SubcategoryID { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int PurchaseInvoiceID { get; set; }
        public virtual PurchaseInvoice PurchaseInvoice { get; set; }
    }
}