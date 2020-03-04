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
    public class OnlineSaleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OnlineSale
        public ActionResult Index()
        {
            if (Session["OutletID"] != null)
            {
                int oid = Convert.ToInt32(Session["OutletID"].ToString());
                List<OnlineInvoiceVM> pivm = new List<OnlineInvoiceVM>();
                var products = (
                                 from inv in db.OnlineInvoices
                                 where inv.OutletID == oid
                                 select new
                                 {
                                     pinvid = inv.OnlineInvoiceID,
                                     partnumber = inv.InvoiceNumber,
                                     status = inv.IsPaid,
                                     bkash = inv.PayerNumber,
                                     trans = inv.TransNumber,
                                     handleby = inv.UserName,
                                     type = inv.PaymentType,
                                     date = inv.OrderDate
                                 }
                                );
                foreach (var item in products)
                {
                    OnlineInvoiceVM pinv = new OnlineInvoiceVM();
                    pinv.OnlineInvoiceID = item.pinvid;
                    pinv.InvoiceNumber = item.partnumber;
                    pinv.IsPaid = item.status;
                    pinv.PayerNumber = item.bkash;
                    pinv.TransNumber = item.trans;
                    pinv.UserName = item.handleby;
                    pinv.PaymentType = item.type;
                    pinv.OrderDate = item.date;
                    pivm.Add(pinv);
                }
                return View(pivm);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult MaxOnlineSale()
        {
            return View();
        }

        // GET: OnlineSale/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSale onlineSale = db.OnlineSales.Find(id);
            if (onlineSale == null)
            {
                return HttpNotFound();
            }
            return View(onlineSale);
        }

        // GET: OnlineSale/Create
        public ActionResult Create()
        {
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemID");
            ViewBag.OnlineInvoiceID = new SelectList(db.OnlineInvoices, "OnlineInvoiceID", "InvoiceNumber");
            return View();
        }

        // POST: OnlineSale/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(string order, string PaymentType, int Outlet, string PayerNumber, string TransNumber, string PaymentMethod, int City, string Address, string ContactNumber, string FullName)
        {
            bool result = false;
            int transID = db.OnlineInvoices.Where(t => t.TransNumber.Trim() == TransNumber.Trim()).Count();
            
            if (Request.IsAuthenticated && transID == 0)
            {
                    var lastInv = db.OnlineInvoices.OrderByDescending(p => p.OnlineInvoiceID).FirstOrDefault();

                    string year = DateTime.Now.Year.ToString();

                    ShippingAddress sa = new ShippingAddress();
                    sa.FullName = FullName;
                    sa.Address = Address;
                    sa.CityID = City;
                    sa.ContactNumber = ContactNumber;
                    db.ShippingAddresses.Add(sa);
                    db.SaveChanges();

                    int lastShipID = sa.ShippingAddressID;

                    OnlineInvoice pmnt = new OnlineInvoice();
                    pmnt.PaymentMethod = PaymentMethod;
                    pmnt.TransNumber = TransNumber;
                    pmnt.UserName = HttpContext.User.Identity.Name;
                    pmnt.PayerNumber = PayerNumber;
                    pmnt.PaymentType = PaymentType;
                    pmnt.OutletID = Outlet;
                    pmnt.ShippingAddressID = lastShipID;
                    pmnt.IsPaid = false;
                    pmnt.IsDeleted = false;
                    pmnt.OrderDate = DateTime.Now;
                    if (lastInv == null)
                    {
                        pmnt.InvoiceNumber = "ONINV" + year + "00001";
                    }
                    else
                    {
                        string invoiceNo = "ONINV" + year + (Convert.ToInt32(lastInv.InvoiceNumber.Substring(9, lastInv.InvoiceNumber.Length - 9)) + 1).ToString("D5");
                        
                        if (lastInv.InvoiceNumber != invoiceNo)
                        {
                            pmnt.InvoiceNumber = invoiceNo;
                        }
                        else
                        {
                            ShippingAddress s = db.ShippingAddresses.Find(lastShipID);
                            db.ShippingAddresses.Remove(s);
                            db.SaveChanges();
                            return Json(result = false, JsonRequestBehavior.AllowGet);
                        }
                    }
                    db.OnlineInvoices.Add(pmnt);
                    db.SaveChanges();

                    int lastPmntID = pmnt.OnlineInvoiceID;

                    OnlineSale[] a = JsonConvert.DeserializeObject<OnlineSale[]>(order);
                    foreach (var item in a)
                    {
                        OnlineSale os = new OnlineSale();
                        os.ItemID = item.ItemID;
                        os.Quantity = item.Quantity;
                        os.OnlineInvoiceID = lastPmntID;
                        os.ItemSize = item.ItemSize;
                        os.ItemPrice = item.ItemPrice;
                        db.OnlineSales.Add(os);
                        db.SaveChanges();
                    }
                    result = true; 
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                result = false;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult StockUpdateWhenApproved(int invID)
        {
            var result = false;
            var products = db.OnlineSales.Where(i => i.OnlineInvoiceID == invID).ToList();
            if(products != null)
            {
                foreach (var item in products)
                {
                    var updateProduct = db.Items.FirstOrDefault(up => up.ItemID == item.ItemID);
                    if (updateProduct != null)
                    {
                        updateProduct.AvailableQnty -= item.Quantity;
                        db.Entry(updateProduct).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var updateInvoice = db.OnlineInvoices.FirstOrDefault(inv=>inv.OnlineInvoiceID==invID);
                if(updateInvoice != null)
                {
                    updateInvoice.IsPaid = true;
                    db.Entry(updateInvoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
                result = true;
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetOutletInvoiceDetails(int invoiceid)
        {
            var items = (
                               from i in db.OnlineSales
                               where i.OnlineInvoiceID == invoiceid
                               select new
                               {
                                   id = i.OnlineInvoiceID,
                                   name = i.Item.Product.ProductName,
                                   size = i.ItemSize,
                                   price = i.ItemPrice,
                                   qnty = i.Quantity,
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

        // GET: OnlineSale/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSale onlineSale = db.OnlineSales.Find(id);
            if (onlineSale == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemID", onlineSale.ItemID);
            ViewBag.OnlineInvoiceID = new SelectList(db.OnlineInvoices, "OnlineInvoiceID", "InvoiceNumber", onlineSale.OnlineInvoiceID);
            return View(onlineSale);
        }

        // POST: OnlineSale/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OnlineSaleID,ItemID,Quantity,OnlineInvoiceID")] OnlineSale onlineSale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(onlineSale).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemID = new SelectList(db.Items, "ItemID", "ItemID", onlineSale.ItemID);
            ViewBag.OnlineInvoiceID = new SelectList(db.OnlineInvoices, "OnlineInvoiceID", "InvoiceNumber", onlineSale.OnlineInvoiceID);
            return View(onlineSale);
        }

        // GET: OnlineSale/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSale onlineSale = db.OnlineSales.Find(id);
            if (onlineSale == null)
            {
                return HttpNotFound();
            }
            return View(onlineSale);
        }

        // POST: OnlineSale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OnlineSale onlineSale = db.OnlineSales.Find(id);
            db.OnlineSales.Remove(onlineSale);
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
