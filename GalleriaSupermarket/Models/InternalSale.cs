using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class InternalSale
    {
        [Key]
        public int InternalSaleID { get; set; }
        [Display(Name = "Invoice No.")]
        public int InvoiceID { get; set; }
        public virtual Invoice Invoice { get; set; }
        [Display(Name = "Item")]
        public int ItemID { get; set; }
        public virtual Item Item { get; set; }
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Display(Name = "Quantity")]
        public int OrderQnty { get; set; }
        public string ItemSize { get; set; }
        public bool IsDeleted { get; set; }
    }
}