using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GalleriaSupermarket.Models;
using Newtonsoft.Json;
using GalleriaSupermarket.Models.ViewModel;

namespace GalleriaSupermarket.Controllers
{
    [Authorize]
    public class InternalSaleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InternalSale
        public ActionResult Index()
        {
            if (Session["OutletID"] != null)
            {
                int oid = Convert.ToInt32(Session["OutletID"].ToString());
                List<InternalInvoiceVm> pivm = new List<InternalInvoiceVm>();
                var products = (
                                 from inv in db.Invoices where inv.OutletID  == oid        
                                    select new
                                    {
                                        pinvid = inv.InvoiceID,
                                        partnumber = inv.PartNumber,
                                        status = inv.IsPaid,
                                        handleby = inv.MadyBy,
                                        type = inv.PaymentType,
                                        date = inv.InvoiceDate
                                    }
                                );
                foreach (var item in products)
                {
                    InternalInvoiceVm pinv = new InternalInvoiceVm();
                    pinv.InvoiceID = item.pinvid;
                    pinv.PartNumber = item.partnumber;
                    pinv.IsPaid = item.status;
                    pinv.MadyBy = item.handleby;
                    pinv.PaymentType = item.type;
                    pinv.InvoiceDate = item.date;
                    pivm.Add(pinv);
                }
                return View(pivm);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public JsonResult GetOutletInvoiceDetails(int invoiceid)
        {
            var items = (
                               from i in db.InternalSales
                               where i.InvoiceID == invoiceid
                               select new
                               {
                                   id = i.InternalSaleID,
                                   name = i.Item.Product.ProductName,
                                   price = i.Price,
                                   size=i.ItemSize,
                                   qnty = i.OrderQnty,
                               }
                           );
            if (items != null)
            {
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        
        // GET: InternalSale/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InternalSale internalSale = db.InternalSales.Find(id);
            if (internalSale == null)
            {
                return HttpNotFound();
            }
            return View(internalSale);
        }

        // GET: InternalSale/Create
        public ActionResult Create()
        {
            if (Session["OutletID"] != null)
            {
                List<LocalSaleVm> lsvm = new List<LocalSaleVm>();
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
                var salesList = db.Items.Where(i => i.OutletID == sid).Include(i => i.Outlet).Include(i => i.Product).Select(p => new { pid = p.ItemID, name = p.Product.ProductName, productid = p.ProductID, price = (p.Product.Price + p.Product.OtherPrice + p.Product.Vat - p.Product.Discount), qnty = p.AvailableQnty });
                foreach (var item in salesList)
                {
                    LocalSaleVm lvm = new LocalSaleVm();
                    lvm.ItemID = item.pid;
                    lvm.ProductName = item.name;
                    lvm.ProductID = item.productid;
                    lvm.Price = item.price;
                    lvm.AvailableQnty = item.qnty;
                    lsvm.Add(lvm);
                }
                return View(lsvm);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // POST: InternalSale/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(string custName, decimal invDiscount, string pmntType, string phone, string order)
        {

            if (Session["OutletID"] != null)
            {
                var lastInv = db.Invoices.OrderByDescending(e => e.InvoiceID).FirstOrDefault();
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
                string year = DateTime.Now.Year.ToString();
                //InternalInvoice
                Invoice inv = new Invoice();
                if (lastInv == null)
                {
                    inv.PartNumber = "LINV" + year + "00001";
                }
                else
                {
                    inv.PartNumber = "LINV" + year + (Convert.ToInt32(lastInv.PartNumber.Substring(8, lastInv.PartNumber.Length - 8)) + 1).ToString("D5");
                }
                inv.CustomerName = custName;
                inv.ContactNumber = phone;
                inv.PaymentType = pmntType;
                inv.MadyBy = HttpContext.User.Identity.Name;
                inv.OutletID = sid;
                inv.InvoiceDiscount = invDiscount;
                inv.IsPaid = true;
                inv.IsDeleted = false;
                inv.InvoiceDate = DateTime.Now;

                db.Invoices.Add(inv);
                db.SaveChanges();

                int lastInvID = inv.InvoiceID;

                InternalSale[] a = JsonConvert.DeserializeObject<InternalSale[]>(order);
                foreach (var item in a)
                {
                    InternalSale sale = new InternalSale();
                    sale.ItemID = item.ItemID;
                    sale.OrderQnty = item.OrderQnty;
                    sale.Price = item.Price;
                    sale.InvoiceID = lastInvID;
                    sale.ItemSize = item.ItemSize;
                    db.InternalSales.Add(sale);

                    var updateItem = db.Items.FirstOrDefault(i=>i.ItemID==item.ItemID);
                    if(updateItem != null)
                    {
                        updateItem.AvailableQnty -= item.OrderQnty;
                        db.Entry(updateItem).State = EntityState.Modified;
                        db.SaveChanges();

                        if (updateItem.AvailableQnty <= 5)
                        {
                            updateItem.IsAvailable = false;
                            db.Entry(updateItem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        
                    }
                    
                }
                db.SaveChanges();

                return Json("saved", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: InternalSale/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InternalSale internalSale = db.InternalSales.Find(id);
            if (internalSale == null)
            {
                return HttpNotFound();
            }
            ViewBag.InvoiceID = new SelectList(db.Invoices, "InvoiceID", "PartNumber", internalSale.InvoiceID);
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemID", internalSale.ItemID);
            return View(internalSale);
        }

        // POST: InternalSale/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InternalSaleID,InvoiceID,ItemID,Price,OrderQnty,IsDeleted")] InternalSale internalSale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(internalSale).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InvoiceID = new SelectList(db.Invoices, "InvoiceID", "PartNumber", internalSale.InvoiceID);
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemID", internalSale.ItemID);
            return View(internalSale);
        }

        // GET: InternalSale/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InternalSale internalSale = db.InternalSales.Find(id);
            if (internalSale == null)
            {
                return HttpNotFound();
            }
            return View(internalSale);
        }

        // POST: InternalSale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InternalSale internalSale = db.InternalSales.Find(id);
            db.InternalSales.Remove(internalSale);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
