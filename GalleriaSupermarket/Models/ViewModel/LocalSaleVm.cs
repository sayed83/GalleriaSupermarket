using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class LocalSaleVm
    {
        public int ItemID { get; set; }
        public int AvailableQnty { get; set; }
        public int OutletID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int SizeID { get; set; }
        public string ItemSize { get; set; }
    }
}