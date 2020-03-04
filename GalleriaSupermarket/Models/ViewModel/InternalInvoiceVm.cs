using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class InternalInvoiceVm
    {
        public int InvoiceID { get; set; }
        public string PartNumber { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public string PaymentType { get; set; }
        public decimal InvoiceDiscount { get; set; }
        public string MadyBy { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime InvoiceDate { get; set; }

        public int InternalSaleID { get; set; }
        public decimal Price { get; set; }
        public decimal OrderQnty { get; set; }
        public int OutletID { get; set; }

    }
}