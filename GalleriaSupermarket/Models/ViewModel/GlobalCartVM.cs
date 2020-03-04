using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class GlobalCartVM
    {
        public int PaymentID { get; set; }
        public int ShippingAddressID { get; set; }
        public int Quantity { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string FullName { get; set; }
        public string PaymentType { get; set; }
        public string PayerNumber { get; set; }
        public string TransNumber { get; set; }
        public string PaymentMethod { get; set; }
        public IEnumerable<OnlineSale> OnlineSales { get; set; }
    }
}