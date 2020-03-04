using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class SalesReturn
    {
        [Key]
        public int SalesReturnID { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "Quantity"), Required]
        public decimal RetrunQnty { get; set; }
        [Display(Name = "Reason"), Required]
        public string RetrunReason { get; set; }
        public int OnlineInvoiceID { get; set; }
        public OnlineInvoice OnlineInvoice { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime ReturnDate { get; set; }
    }
}