using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class OnlineInvoiceVM
    {
        public int OnlineSaleID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public int OnlineInvoiceID { get; set; }
        public string ItemSize { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentType { get; set; }
        public string PayerNumber { get; set; }
        public string TransNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string UserName { get; set; }
        public int ShippingAddressID { get; set; }
        public int OutletID { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }
    }
}