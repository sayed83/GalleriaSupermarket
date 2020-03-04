using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class CustomerOrderVM
    {
        public int OnlineInvoiceID { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderDate { get; set; }
        public bool IsPaid { get; set; }
        public decimal ItemPrice { get; set; }
        public bool IsDeleted { get; set; }
    }
}