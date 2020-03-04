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
    public class OperationCostDetailController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OperationCostDetail
        public ActionResult Index()
        {
            var operationCostDetails = db.OperationCostDetails.Include(o => o.Expense);
            return View(operationCostDetails.ToList());
        }

        // GET: OperationCostDetail/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationCostDetail operationCostDetail = db.OperationCostDetails.Find(id);
            if (operationCostDetail == null)
            {
                return HttpNotFound();
            }
            return View(operationCostDetail);
        }

        // GET: OperationCostDetail/Create
        public ActionResult Create()
        {
            if(Session["OutletID"] != null)
            {
                ViewBag.Month = DateTime.Now.ToString("MMMM");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }            
        }

        // POST: OperationCostDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OperationCostDetailID,CostHead,ExpenseID,CostAmount,AddedBy,CostDate")] OperationCostDetail operationCostDetail)
        {
            if (ModelState.IsValid)
            {
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
                string month = DateTime.Now.ToString("MMMM");
                int expID = db.Expenses.FirstOrDefault(e => e.OutletID == sid && e.Month.ToLower() == month.ToLower()).ExpenseID;
                operationCostDetail.ExpenseID = expID;
                operationCostDetail.AddedBy = HttpContext.User.Identity.Name;
                operationCostDetail.CostDate = DateTime.Now;
                db.OperationCostDetails.Add(operationCostDetail);
                db.SaveChanges();

                var expenseUpdate = db.Expenses.Where(e => e.ExpenseID == expID).FirstOrDefault();
                if(operationCostDetail.CostAmount > expenseUpdate.CashOnHand)
                {
                    if(expenseUpdate.CashOnHand == 0)
                    {
                        expenseUpdate.OtherExpense += operationCostDetail.CostAmount;
                    }
                    else
                    {
                        expenseUpdate.OtherExpense = operationCostDetail.CostAmount - expenseUpdate.CashOnHand;
                        expenseUpdate.CashOnHand -= expenseUpdate.CashOnHand;
                    }
                    
                }
                else
                {
                    expenseUpdate.CashOnHand -= operationCostDetail.CostAmount;
                }
                db.Entry(expenseUpdate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ExpenseID = new SelectList(db.Expenses, "ExpenseID", "Month", operationCostDetail.ExpenseID);
            return View(operationCostDetail);
        }

        // GET: OperationCostDetail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationCostDetail operationCostDetail = db.OperationCostDetails.Find(id);
            if (operationCostDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExpenseID = new SelectList(db.Expenses, "ExpenseID", "Month", operationCostDetail.ExpenseID);
            return View(operationCostDetail);
        }

        // POST: OperationCostDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OperationCostDetailID,CostHead,ExpenseID,CostAmount,AddedBy,CostDate")] OperationCostDetail operationCostDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(operationCostDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ExpenseID = new SelectList(db.Expenses, "ExpenseID", "Month", operationCostDetail.ExpenseID);
            return View(operationCostDetail);
        }

        // GET: OperationCostDetail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OperationCostDetail operationCostDetail = db.OperationCostDetails.Find(id);
            if (operationCostDetail == null)
            {
                return HttpNotFound();
            }
            return View(operationCostDetail);
        }

        // POST: OperationCostDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OperationCostDetail operationCostDetail = db.OperationCostDetails.Find(id);
            db.OperationCostDetails.Remove(operationCostDetail);
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
