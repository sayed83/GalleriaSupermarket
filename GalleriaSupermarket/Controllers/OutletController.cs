using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GalleriaSupermarket.Models;

namespace GalleriaSupermarket.Controllers
{
    public class OutletController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Outlet
        public ActionResult Index()
        {
            var outlets = db.Outlets.Where(o=> o.IsDeleted == false).Include(o => o.City);
            return View(outlets.ToList());
        }

        public JsonResult GetOutlet()
        {
            int sid = Convert.ToInt32(Session["OutletID"].ToString());
            var outlets = db.Outlets.Where(o => o.OutletID != sid).AsEnumerable().Select(o=>new { id=o.OutletID, name=o.OutletName}) ;
            if(outlets != null)
            {
                return Json(outlets, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Outlet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Outlet outlet = db.Outlets.Find(id);
            if (outlet == null)
            {
                return HttpNotFound();
            }
            return View(outlet);
        }

        // GET: Outlet/Create
        public ActionResult Create()
        {
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            return View();
        }

        // POST: Outlet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OutletID,OutletName,ShortAddress,DetailsAddress,CreationDate,CityID,IsDeleted")] Outlet outlet)
        {
            if (ModelState.IsValid)
            {
                int ckoutlet = db.Outlets.Where(o => o.OutletName == outlet.OutletName).Count();
                if (ckoutlet > 0)
                {
                    ModelState.AddModelError("","Outlet Already Exits!");
                }
                else
                {
                    outlet.CreationDate = DateTime.Now.Date;
                    outlet.IsDeleted = false;
                    db.Outlets.Add(outlet);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                
            }

            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", outlet.CityID);
            return View(outlet);
        }

        // GET: Outlet/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Outlet outlet = db.Outlets.Find(id);
            if (outlet == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", outlet.CityID);
            return View(outlet);
        }

        // POST: Outlet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OutletID,OutletName,ShortAddress,DetailsAddress,CreationDate,CityID,IsDeleted")] Outlet outlet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(outlet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", outlet.CityID);
            return View(outlet);
        }

        // GET: Outlet/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Outlet outlet = db.Outlets.Find(id);
            if (outlet == null)
            {
                return HttpNotFound();
            }
            return View(outlet);
        }

        // POST: Outlet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Outlet outlet = db.Outlets.Find(id);
            db.Outlets.Remove(outlet);
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
