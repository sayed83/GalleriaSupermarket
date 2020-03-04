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
    public class ItemTransferController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ItemTransfer
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTransferItemInOutlet()
        {
            if (HttpContext.User.IsInRole("Admin"))
            {
                var itemTransfers = db.ItemTransfers.AsEnumerable().Select(i => new { id = i.ItemTransferID, managerApproval= i.ManagerApproved, date=i.TransferDate.ToString("MMM dd, yyyy"), outlet = i.Outlet.OutletName, from= db.Outlets.FirstOrDefault(o=>o.OutletID == i.ItemFrom).OutletName });

                if(itemTransfers != null)
                {
                    return Json(itemTransfers, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
                var itemTransfers = db.ItemTransfers.Where(o => o.OutletID == sid).AsEnumerable().Select(i => new { id = i.ItemTransferID, managerApproval = i.ManagerApproved, date = i.TransferDate.ToString("MMM dd, yyyy"), outlet = i.Outlet.OutletName, from = db.Outlets.FirstOrDefault(o => o.OutletID == i.ItemFrom).OutletName });

                if (itemTransfers != null)
                {
                    return Json(itemTransfers, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            
        }

        public JsonResult GetItemTransferRequest() {

            int listRequest = db.ItemTransfers.Where(i => i.ManagerApproved == false).Count();
            if(listRequest > 0)
            {
                return Json(listRequest, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: ItemTransfer/Create
        public ActionResult Create()
        {
            if(Session["OutletID"] != null)
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


        // POST: ItemTransfer/Create
        [HttpPost]
        public JsonResult Create(string transItem, int OutletId)
        {
            int sid = Convert.ToInt32(Session["OutletID"].ToString());
            
            if(OutletId != 0 && transItem != null)
            {
                ItemTransfer itemTransfer = new ItemTransfer();
                itemTransfer.ManagerApproved = false;
                itemTransfer.ItemFrom = sid;
                itemTransfer.Item = transItem;
                itemTransfer.OutletID = OutletId;
                itemTransfer.IsDeleted = false;
                itemTransfer.TransferDate = DateTime.Now;
                db.ItemTransfers.Add(itemTransfer);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            
        }

        // Update Stock based on Manager Approval 
        public JsonResult UpdateItemQntyManagerApproval(int transferId)
        {
            if(Session["OutletID"] != null)
            {
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
                string itemTransfers = db.ItemTransfers.FirstOrDefault(it => it.ItemTransferID == transferId).Item;
                Item[] i = JsonConvert.DeserializeObject<Item[]>(itemTransfers);
                foreach (var item in i)
                {
                    var updateItem = db.Items.Find(item.ItemID);
                    if (updateItem != null)
                    {
                        updateItem.AvailableQnty -= item.AvailableQnty;
                        db.Entry(updateItem).State = EntityState.Modified;
                        db.SaveChanges();

                        var increaseQnty = db.Items.FirstOrDefault(ic => ic.ProductID == db.Items.FirstOrDefault(q=>q.ItemID ==item.ItemID).ProductID && ic.OutletID == sid);
                        increaseQnty.AvailableQnty += item.AvailableQnty;
                        db.Entry(increaseQnty).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }

                var updateStatus = db.ItemTransfers.FirstOrDefault(st => st.ItemTransferID == transferId);
                if(updateStatus != null)
                {
                    updateStatus.ManagerApproved = true;
                    db.Entry(updateStatus).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            } 
        }


        public JsonResult GetItemTransferDetails(int ItemTransId)
        {
            if (HttpContext.User.IsInRole("Admin"))
            {
                string itemTransfers = db.ItemTransfers.FirstOrDefault(it=>it.ItemTransferID == ItemTransId).Item;
                
                if (itemTransfers != null)
                {
                    return Json(itemTransfers, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
                string itemTransfers = db.ItemTransfers.FirstOrDefault(it => it.ItemTransferID == ItemTransId && it.OutletID == sid).Item;

                if (itemTransfers != null)
                {
                    return Json(itemTransfers, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
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
            if (product != null)
            {
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
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
