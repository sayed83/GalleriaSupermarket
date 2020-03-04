using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class TransferItemVM
    {
        public int ItemID { get; set; }
        public int AvailableQnty { get; set; }
        public string ItemSize { get; set; }
    }
}