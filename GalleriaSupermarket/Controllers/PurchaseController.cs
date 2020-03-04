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
    public class PurchaseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Purchase
        public ActionResult Index()
        {
            List<PurchaseInvoiceVm> pivm = new List<PurchaseInvoiceVm>();
            var products = (from pi in db.PurchaseInvoices
                            join s in db.Suppliers
                            on pi.SupplierID equals s.SupplierID
                            join o in db.Outlets
                            on pi.OutletID equals o.OutletID
                            select new
                            {
                                pinvid = pi.PurchaseInvoiceID,
                                partnumber = pi.PartNumber,
                                total = pi.TotalPrice,
                                paidamnt = pi.PaidAmount,
                                dueamnt= pi.DueAmount,
                                status = pi.IsPaid,
                                outlet= o.OutletName,
                                Supplier= s.CompanyName,
                                date=pi.InvoiceDate
                            });
            foreach (var item in products)
            {
                PurchaseInvoiceVm pinv = new PurchaseInvoiceVm();
                pinv.PurchaseInvoiceID = item.pinvid;
                pinv.PartNumber = item.partnumber;
                pinv.TotalPrice = item.total;
                pinv.PaidAmount = item.paidamnt;
                pinv.DueAmount = item.dueamnt;
                pinv.IsPaid = item.status;
                pinv.OutletName = item.outlet;
                pinv.CompanyName = item.Supplier;
                pinv.InvoiceDate = item.date.ToString("MMM dd, yyyy");
                pivm.Add(pinv);
            }
            return View(pivm);
        }
        public ActionResult SupplierPayment()
        {
            return View();
        }

        public JsonResult duePaidToSupplier(int InvoiceId, int supplierId, decimal payAmount)
        {
            if(InvoiceId != 0)
            {
                var invoiceExists = db.PurchaseInvoices.FirstOrDefault(i => i.PurchaseInvoiceID == InvoiceId && i.SupplierID == supplierId);
                if(invoiceExists != null)
                {
                    
                    if(invoiceExists.DueAmount > payAmount)
                    {
                        invoiceExists.PaidAmount += payAmount;
                        invoiceExists.DueAmount -= payAmount;
                        
                    }
                    else if(invoiceExists.DueAmount == payAmount)
                    {
                        invoiceExists.IsPaid = true;
                        invoiceExists.PaidAmount += payAmount;
                        invoiceExists.DueAmount -= payAmount;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Not more then due amount");
                    }
                    db.Entry(invoiceExists).State = EntityState.Modified;
                    db.SaveChanges();

                    SupplierPayment spmnt = new SupplierPayment();
                    spmnt.PurchaseInvoiceID = InvoiceId;
                    spmnt.PaidAmount = payAmount;
                    spmnt.PaymentDate = DateTime.Now.Date;
                    spmnt.IsDeleted = false;
                    db.SupplierPayments.Add(spmnt);
                    db.SaveChanges();
                }
                return Json("Saved", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetDueInvoiceBySpplier(int id)
        {
            var dueInvoice = (
                                from pi in db.PurchaseInvoices
                                join o in db.Outlets
                                on pi.OutletID equals o.OutletID
                                where pi.SupplierID == id && pi.IsPaid == false
                                select new
                                {
                                    id = pi.PurchaseInvoiceID,
                                    supId=pi.SupplierID,
                                    invoice = pi.PartNumber,
                                    dueamnt = pi.DueAmount,
                                    outlet = o.OutletName
                                }
                              );
            if(dueInvoice != null)
            {
                return Json(dueInvoice, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetSupplier()
        {
            var supplierList = db.Suppliers.AsEnumerable().Select(s => new { id = s.SupplierID, name = s.CompanyName });
            if(supplierList != null)
            {
                return Json(supplierList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetInvoiceDetails(int invoiceid)
        {
            var product = (
                               from p in db.Purchases
                               join sc in db.SubCategories
                               on p.SubcategoryID equals sc.SubCategoryID
                               where p.PurchaseInvoiceID == invoiceid
                               select new
                               {
                                   id = p.PurchaseID,
                                   name = sc.SubCategoryName,
                                   price = p.Price,
                                   qnty = p.Quantity
                               }
                           );
            if(product != null)
            {
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetOutlets()
        {
            var outletList = db.Outlets.AsEnumerable().Select(o => new { id = o.OutletID, name = o.OutletName });
            if(outletList != null)
            {
                return Json(outletList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
           
        }

        // GET: Purchase/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // GET: Purchase/Create
        public ActionResult Create()
        {
            List<BrandVm> BrandList = new List<BrandVm>();
            var brands = db.Brands.ToList();
            foreach (var item in brands)
            {
                BrandVm obj = new BrandVm();
                obj.BrandID = item.BrandID;
                obj.BrandName = item.BrandName;
                BrandList.Add(obj);
            }

            return View(BrandList);
        }

        // POST: Purchase/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(int supplierId, int outletId, decimal totalAmnt, decimal shipCost, decimal paidamnt, decimal dueamnt, decimal netAmount, string purOrder)
        {

            if (supplierId != 0)
            {
                var lastInv = db.PurchaseInvoices.OrderByDescending(e => e.PurchaseInvoiceID).FirstOrDefault();
                string year = DateTime.Now.Year.ToString();
                PurchaseInvoice inv = new PurchaseInvoice();
                if (lastInv == null)
                {
                    inv.PartNumber = "PINV" + year + "00001";
                }
                else
                {
                    inv.PartNumber = "PINV" + year + (Convert.ToInt32(lastInv.PartNumber.Substring(8, lastInv.PartNumber.Length - 8)) + 1).ToString("D5");
                }
                inv.TotalPrice = totalAmnt;
                inv.PaidAmount = paidamnt;
                inv.DueAmount = dueamnt;
                inv.NetAmount = netAmount;
                inv.ShppingCost = shipCost;
                inv.OutletID = outletId;
                inv.SupplierID = supplierId;
                inv.InvoiceDate = DateTime.Now.Date;

                if(totalAmnt != paidamnt)
                {
                    inv.IsPaid = false;
                }
                else
                {
                    inv.IsPaid = true;
                }
                inv.IsDeleted = false;

                db.PurchaseInvoices.Add(inv);
                db.SaveChanges();

                int lastInvID = inv.PurchaseInvoiceID;

                Purchase[] a = JsonConvert.DeserializeObject<Purchase[]>(purOrder);
                foreach (var item in a)
                {
                    Purchase pur = new Purchase();
                    pur.SubcategoryID = item.SubcategoryID;
                    pur.Price = item.Price;
                    pur.Quantity = item.Quantity;
                    pur.PurchaseInvoiceID = lastInvID;
                    db.Purchases.Add(pur);
                 }
                db.SaveChanges();

                return Json("saved", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: Purchase/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.PurchaseInvoiceID = new SelectList(db.PurchaseInvoices, "PurchaseInvoiceID", "PartNumber", purchase.PurchaseInvoiceID);
            ViewBag.SubcategoryID = new SelectList(db.SubCategories, "SubCategoryID", "SubCategoryName", purchase.SubcategoryID);
            return View(purchase);
        }

        // POST: Purchase/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseID,SubcategoryID,Price,Quantity,PurchaseInvoiceID")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PurchaseInvoiceID = new SelectList(db.PurchaseInvoices, "PurchaseInvoiceID", "PartNumber", purchase.PurchaseInvoiceID);
            ViewBag.SubcategoryID = new SelectList(db.SubCategories, "SubCategoryID", "SubCategoryName", purchase.SubcategoryID);
            return View(purchase);
        }

        // GET: Purchase/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // POST: Purchase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            db.Purchases.Remove(purchase);
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
