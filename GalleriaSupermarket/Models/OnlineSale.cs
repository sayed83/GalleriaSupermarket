using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class OnlineSale
    {
        [Key]
        public int OnlineSaleID { get; set; }
        public int ItemID { get; set; }
        public virtual Item Item { get; set; }
        public int Quantity { get; set; }
        public int OnlineInvoiceID { get; set; }
        public string ItemSize { get; set; }
        public decimal ItemPrice { get; set; }
        public virtual OnlineInvoice OnlineInvoice { get; set; }
        
    }
}