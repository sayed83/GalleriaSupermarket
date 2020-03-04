using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GalleriaSupermarket.Models;
using GalleriaSupermarket.Models.ViewModel;

namespace GalleriaSupermarket.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Item
        public ActionResult Index()
        {
            if(Session["OutletID"] != null)
            {
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
                List<ItemVm> itemList = new List<ItemVm>();
                var queryItem = db.Items.Where(i=>i.OutletID == sid).Include(i => i.Outlet).Include(i => i.Product).AsEnumerable().Select(i => new { id = i.ItemID, name = i.Product.ProductName, outlet = i.Outlet.OutletName, qnty = i.AvailableQnty, status = i.IsAvailable });
                foreach (var item in queryItem)
                {
                    ItemVm ivm = new ItemVm();
                    ivm.ItemID = item.id;
                    ivm.ProductName = item.name;
                    ivm.OutletName = item.outlet;
                    ivm.AvailableQnty = item.qnty;
                    ivm.IsAvailable = item.status;
                    itemList.Add(ivm);
                }
                return View(itemList);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        // GET: Item/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Item/Create
        public ActionResult Create()
        {
            ViewBag.OutletID = new SelectList(db.Outlets, "OutletID", "OutletName");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }
        public ActionResult OutletItem()
        {
            if(Session["OutletID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        public ActionResult LostProudct()
        {
            if(Session["OutletID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        public JsonResult GetLostProuduct()
        {
            int sid = Convert.ToInt32(Session["OutletID"].ToString());
            var lostproductList = db.LostProducts.Where(o => o.OutletID == sid).AsEnumerable().Select(lp => new { id = lp.LostProductID, name = lp.Item.Product.ProductName, qnty = lp.LostQnty, date = lp.AddedDate.ToString("MMM dd, yyyy"), username = lp.AddedBy, status = lp.IsDeleted });

            if (lostproductList != null)
            {
                return Json(lostproductList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult ProductRequest()
        {
            if(Session["OutletID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public JsonResult SendProductRequest(string requestText)
        {
            if(requestText != null)
            {
                ProductRequest pr = new ProductRequest();
                pr.OutletId = Convert.ToInt32(Session["OutletID"].ToString());
                pr.UserName = HttpContext.User.Identity.Name;
                pr.RequestDate = DateTime.Now;
                pr.Description = requestText;
                pr.Status = false;
                pr.IsDeleted = false;
                db.ProductRequests.Add(pr);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult GetProductRequest()
        {
            if (!HttpContext.User.IsInRole("Admin"))
            {
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
               var getRequest = db.ProductRequests.Where(pr => pr.OutletId == sid && pr.IsDeleted == false).AsEnumerable().Select(pr => new { id = pr.ProductRequestID, name = pr.Outlet.OutletName, manager = pr.UserName, approved = pr.Status, date=pr.RequestDate.ToString("MMM dd, yyyy") });
                if(getRequest != null)
                {
                    return Json(getRequest, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                
            }
            else
            {
               var getRequest = db.ProductRequests.Where(pr => pr.IsDeleted == false).AsEnumerable().Select(pr => new { id = pr.ProductRequestID, name = pr.Outlet.OutletName, manager = pr.UserName, approved = pr.Status, date = pr.RequestDate.ToString("MMM dd, yyyy") });
                if (getRequest != null)
                {
                    return Json(getRequest, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            
        }


        public JsonResult GetProductRequestDetails(int requestId)
        {
            var reqText = db.ProductRequests.Where(r => r.ProductRequestID == requestId).AsEnumerable().Select(r => new { id=r.ProductRequestID, details = r.Description });
            if(reqText != null)
            {
                return Json(reqText, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ApprovedProRequestByAdmin(int rid)
        {
            var getRequest = db.ProductRequests.Where(r => r.ProductRequestID == rid).SingleOrDefault();
            if(getRequest != null)
            {
                getRequest.Status = true;
                db.Entry(getRequest).State = EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult OwnOutletItem()
        {
            string id = Session["OutletID"].ToString();
            int sid = Convert.ToInt32(id);
            var items = db.Items.Where(i=>i.OutletID == sid).Include(i => i.Outlet).Include(i => i.Product).AsEnumerable().Select(i => new { id = i.ItemID, name = i.Product.ProductName, outlet = i.Outlet.OutletName, qnty = i.AvailableQnty, status = i.IsAvailable });
            if (items != null)
            {
                return Json(items, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AutoCompleteLostItem(string name)
        {
            int sid = Convert.ToInt32(Session["OutletID"].ToString());
            var product = from i in db.Items
                          where i.Product.ProductName.Contains(name) && i.OutletID == sid
                          select new
                          {
                              id = i.ItemID,
                              name = i.Product.ProductName
                          };
            if(product != null)
            {
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveLostProduct(int itemId, int qnty)
        {
            if (qnty > 0)
            {
                LostProduct lp = new LostProduct();
                lp.ItemID = itemId;
                if(qnty > 0)
                {
                    lp.LostQnty = qnty;

                    var ckItem = db.Items.FirstOrDefault(i => i.ItemID == itemId);
                    if(ckItem != null)
                    {
                        if(qnty < ckItem.AvailableQnty)
                        {
                            ckItem.AvailableQnty -= qnty;
                            db.Entry(ckItem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                lp.OutletID = Convert.ToInt32(Session["OutletID"].ToString());
                lp.AddedBy = HttpContext.User.Identity.Name;
                lp.AddedDate = DateTime.Now;
                lp.IsDeleted = false;
                db.LostProducts.Add(lp);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Item/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemID,ProductID,OutletID,AvailableQnty,IsAvailable,IsDeleted")] Item item)
        {
            if (Session["OutletID"] != null)
            {
                int oid = Convert.ToInt32(Session["OutletID"].ToString());
                                
                var checkItem = db.Items.FirstOrDefault(i => i.ProductID == item.ProductID && i.OutletID == oid);
                if (checkItem != null)
                {
                    checkItem.OutletID = oid;
                    checkItem.IsAvailable = item.IsAvailable;
                    checkItem.ProductID = item.ProductID;
                    checkItem.AvailableQnty += item.AvailableQnty;
                    db.Entry(checkItem).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        item.OutletID = oid;
                        item.IsAvailable = true;
                        item.IsDeleted = false;
                        db.Items.Add(item);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View(item);
        }

        // GET: Item/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.OutletID = new SelectList(db.Outlets, "OutletID", "OutletName", item.OutletID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", item.ProductID);
            return View(item);
        }

        // POST: Item/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemID,ProductID,OutletID,AvailableQnty,IsAvailable,IsDeleted")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OutletID = new SelectList(db.Outlets, "OutletID", "OutletName", item.OutletID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", item.ProductID);
            return View(item);
        }
        
        public JsonResult QuantityRowUpdate(int id, int Qnty)
        {
            var item = db.Items.FirstOrDefault(i => i.ItemID == id);
            if(item != null)
            {
                if(Qnty > 0) {
                    int qnty = item.AvailableQnty += Qnty;
                    if(qnty > 5)
                    {
                        item.AvailableQnty = qnty;
                        item.IsAvailable = true;
                    }
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    return Json("zeroQnty", JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Item/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
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
