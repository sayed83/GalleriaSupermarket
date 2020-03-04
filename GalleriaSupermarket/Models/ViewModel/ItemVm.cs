using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class ItemVm
    {
        public int ItemID { get; set; }
        public decimal AvailableQnty { get; set; }
        public bool IsAvailable { get; set; }

        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public int OutletID { get; set; }
        public string OutletName { get; set; }
    }
}