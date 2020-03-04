using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using GalleriaSupermarket.Models;
using GalleriaSupermarket.Models.ViewModel;

namespace GalleriaSupermarket.Controllers
{
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Report
        public ActionResult SalesReport()
        {
            List<SaleReportVm> srvm = new List<SaleReportVm>();

            var onlineSale = db.OnlineSales.AsEnumerable().Select(os => new { productName = os.Item.Product.ProductName, saleDate = os.OnlineInvoice.OrderDate, outletName = os.OnlineInvoice.Outlet.OutletName, totalSale = os.ItemPrice, saleType = "Online" });

            var localSale = db.InternalSales.AsEnumerable().Select(ins => new { productName = ins.Item.Product.ProductName, saleDate = ins.Invoice.InvoiceDate, outletName = ins.Invoice.Outlet.OutletName, totalSale = ins.Price, saleType = "Local" });

            var query = onlineSale.Concat(localSale);//joining two above query into new query

            var filterQuery = query.GroupBy(i => new { i.productName, i.saleDate, i.outletName, i.saleType }).Select(g => new { productName = g.Key.productName, saleDate = g.Key.saleDate, outletName = g.Key.outletName, totalSale = g.Sum(i => i.totalSale), saleType = g.Key.saleType });

            foreach (var item in filterQuery)
            {
                SaleReportVm obj = new SaleReportVm();
                obj.ProductName = item.productName;
                obj.Date = item.saleDate.ToString("MMM dd, yyyy");
                obj.OutletName = item.outletName;
                obj.TotalSale = item.totalSale.ToString();
                obj.SaleType = item.saleType;
                srvm.Add(obj);
            }            
            return View(srvm);
        }
        
        public ActionResult SalesReportWithOptions(string fromDate, string toDate)
        {
            DateTime fDate = Convert.ToDateTime(fromDate);
            DateTime tDate = Convert.ToDateTime(toDate);
            List<SaleReportVm> srvm = new List<SaleReportVm>();

            var onlineSale = db.OnlineSales.AsEnumerable().Select(os => new { productName = os.Item.Product.ProductName, saleDate = os.OnlineInvoice.OrderDate, outletName = os.OnlineInvoice.Outlet.OutletName, totalSale = os.ItemPrice, saleType = "Online" });

            var localSale = db.InternalSales.AsEnumerable().Select(ins => new { productName = ins.Item.Product.ProductName, saleDate = ins.Invoice.InvoiceDate, outletName = ins.Invoice.Outlet.OutletName, totalSale = ins.Price, saleType = "Local" });

            var query = onlineSale.Concat(localSale);//joining two above query into new query

            if (!String.IsNullOrEmpty(fromDate) && !String.IsNullOrEmpty(toDate))
            {
                query = query.Where(i => i.saleDate >= fDate && i.saleDate <= fDate).GroupBy(i => new { i.productName, i.saleDate, i.outletName, i.saleType }).Select(g => new { productName = g.Key.productName, saleDate = g.Key.saleDate, outletName = g.Key.outletName, totalSale = g.Sum(i => i.totalSale), saleType = g.Key.saleType });
            }
            else
            {
                query = query.GroupBy(i => new { i.productName, i.saleDate, i.outletName, i.saleType }).Select(g => new { productName = g.Key.productName, saleDate = g.Key.saleDate, outletName = g.Key.outletName, totalSale = g.Sum(i => i.totalSale), saleType = g.Key.saleType });
            }
                        
            foreach (var item in query)
            {
                SaleReportVm obj = new SaleReportVm();
                obj.ProductName = item.productName;
                obj.Date = item.saleDate.ToString("MMM dd, yyyy");
                obj.OutletName = item.outletName;
                obj.TotalSale = item.totalSale.ToString();
                obj.SaleType = item.saleType;
                srvm.Add(obj);
            }
            return View(srvm);
        }

        public JsonResult GetOutlet() {
            var outletList = db.Outlets.AsEnumerable().Select(o => new { id = o.OutletID, name = o.OutletName });
            if(outletList != null)
            {
                return Json(outletList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(outletList, JsonRequestBehavior.AllowGet);
            }
        }
    }
}