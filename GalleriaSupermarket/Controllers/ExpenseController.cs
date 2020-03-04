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
    public class ExpenseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Expense
        public ActionResult Index()
        {
            var expenses = db.Expenses.Include(e => e.Outlet);
            return View(expenses.ToList());
        }

        // GET: Expense/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }
        public ActionResult GetOutletExpense()
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
        public JsonResult GetOutletExpenseHistory()
        {
            int sid = Convert.ToInt32(Session["OutletID"].ToString());
            var expenses = db.Expenses.Where(e => e.OutletID == sid).AsEnumerable().Select(e => new { id = e.ExpenseID, date = e.ExpenseIssueDate.ToString(), elec = e.ElectricityBill, operation = e.OutletOperationCost, month=e.Month, other = e.OtherExpense, cash=e.CashOnHand, salary = e.OutletStaffSalary, status = e.IsReceived, action = e.IsDeleted });
            if(expenses != null)
            {
                return Json(expenses, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult StatusChange(int eid)
        {
            var cngStatus = db.Expenses.Where(e => e.ExpenseID == eid).FirstOrDefault();
            if(cngStatus != null)
            {
                cngStatus.IsReceived = true;
                db.Entry(cngStatus).State = EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetOperationCostDetails(int expId)
        {
            var costDetails = db.OperationCostDetails.Where(c => c.ExpenseID == expId).AsEnumerable().Select(cd => new { id = cd.OperationCostDetailID, head = cd.CostHead, costBy = cd.AddedBy, amnt = cd.CostAmount, date = cd.CostDate.ToString("MMM dd, yyyy") });
            if(costDetails != null)
            {
                return Json(costDetails, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Expense/Create
        public ActionResult Create()
        {
            ViewBag.OutletID = new SelectList(db.Outlets, "OutletID", "OutletName");
            return View();
        }

        // POST: Expense/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExpenseID,ElectricityBill,OutletOperationCost,OutletID,OutletStaffSalary,OtherExpense,ExpenseIssueDate,Month,IsReceived,IsDeleted,CashOnHand")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                expense.ExpenseIssueDate = DateTime.Now;
                decimal cashonhand = expense.OutletOperationCost;
                expense.CashOnHand = cashonhand;
                expense.IsDeleted = false;
                db.Expenses.Add(expense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OutletID = new SelectList(db.Outlets, "OutletID", "OutletName", expense.OutletID);
            return View(expense);
        }

        // GET: Expense/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            ViewBag.OutletID = new SelectList(db.Outlets, "OutletID", "OutletName", expense.OutletID);
            return View(expense);
        }

        // POST: Expense/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExpenseID,ElectricityBill,OutletOperationCost,OutletID,OutletStaffSalary,OtherExpense,ExpenseIssueDate,Month,IsReceived,IsDeleted")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                db.Entry(expense).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OutletID = new SelectList(db.Outlets, "OutletID", "OutletName", expense.OutletID);
            return View(expense);
        }

        // GET: Expense/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Expense expense = db.Expenses.Find(id);
            db.Expenses.Remove(expense);
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
