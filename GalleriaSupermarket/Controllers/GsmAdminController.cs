using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GalleriaSupermarket.Models;
using System.Data.Entity;

namespace GalleriaSupermarket.Controllers
{
    [Authorize(Roles ="Admin, Manager, Salesman")]
    public class GsmAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext(); 
        // GET: GsmAdmin
        public ActionResult Index()
        {
            if (!HttpContext.User.IsInRole("Admin"))
            {
                if (Session["OutletID"] !=null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        public JsonResult GetUserInfo()
        {
            int userList = db.Users.Where(u => u.UserType == "Customer").Count();
            if(userList > 0)
            {
                return Json(userList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetTodayOrder()
        {
            var today = DateTime.Now.Date;
            int orderCount = db.OnlineInvoices.Where(o=> DbFunctions.TruncateTime(o.OrderDate) == today).Count();
            
            if (orderCount > 0)
            {
                return Json(orderCount, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }


    }
}