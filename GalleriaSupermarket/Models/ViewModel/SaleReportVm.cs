using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class SaleReportVm
    {
        public string ProductName { get; set; }
        public string Date { get; set; }
        public string OutletName { get; set; }
        public string TotalSale { get; set; }
        public string SaleType { get; set; }
    }
}