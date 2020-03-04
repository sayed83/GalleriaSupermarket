using GalleriaSupermarket.Models;
using GalleriaSupermarket.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleriaSupermarket.Controllers
{
    [Authorize]
    public class MyAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: MyAccount
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyOrder()
        {
            List<CustomerOrderVM> covm = new List<CustomerOrderVM>();
            var orderHistoryList = db.OnlineInvoices.Where(o => o.UserName == HttpContext.User.Identity.Name).Include(os => os.OnlineSales).AsEnumerable().Select(o => new { id = o.OnlineInvoiceID, invNumber = o.InvoiceNumber, price = db.OnlineSales.Where(s => s.OnlineInvoiceID == o.OnlineInvoiceID).Sum(s => s.ItemPrice), status = o.IsPaid, date = o.OrderDate.ToString("MMM dd, yyyy") });
            foreach (var item in orderHistoryList)
            {
                CustomerOrderVM obj = new CustomerOrderVM();
                obj.OnlineInvoiceID = item.id;
                obj.InvoiceNumber = item.invNumber;
                obj.ItemPrice = item.price;
                obj.OrderDate = item.date;
                obj.IsPaid = item.status;
                covm.Add(obj);
            }

            return View(covm);
        }

        public JsonResult GetOrders()
        {
            var orderHistoryList = db.OnlineInvoices.Where(o => o.UserName == HttpContext.User.Identity.Name).Include(os => os.OnlineSales).AsEnumerable().Select(o => new { id = o.OnlineInvoiceID, invNumber = o.InvoiceNumber, price = db.OnlineSales.Where(s => s.OnlineInvoiceID == o.OnlineInvoiceID).Sum(s => s.ItemPrice), status = o.IsPaid, date = o.OrderDate.ToString("MMM dd, yyyy") });
            if(orderHistoryList != null)
            {
                return Json(orderHistoryList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult UserProfile()
        {
            return View();
        }

        public JsonResult GetUserData()
        {
            var userData = db.Users.Where(u => u.UserName == HttpContext.User.Identity.Name).AsEnumerable().Select(u => new { id = u.Id, image = u.ImageUrl, name = u.FullName, email = u.Email, username = u.UserName, role = u.UserType }).SingleOrDefault();
            if (userData != null)
            {
                return Json(userData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UserProfile(ApplicationUser userInfo)
        {
            var userData = db.Users.Where(u => u.UserName == HttpContext.User.Identity.Name).SingleOrDefault();
            if (userData != null)
            {
                if (userInfo.UserImageUpload != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(userInfo.UserImageUpload.FileName);
                    string extension = Path.GetExtension(userInfo.UserImageUpload.FileName);
                    string oldImg = userData.ImageUrl;
                    filename = filename + "_" + DateTime.Now.ToString("yymmssfff") + extension;
                    userData.FullName = userInfo.FullName;
                    userData.ImageUrl = filename;

                    db.Entry(userData).State = EntityState.Modified;
                    db.SaveChanges();
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/App_File/CustomerImg"), oldImg));
                    userInfo.UserImageUpload.SaveAs(Path.Combine(Server.MapPath("~/App_File/Staff_Img"), filename));
                    return RedirectToAction("UserProfile", "Account");
                }
                else
                {
                    userData.FullName = userInfo.FullName;
                    db.Entry(userData).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return View(userInfo);
            }
            else
            {
               ModelState.AddModelError("", "Something Went Wrong!");
                return View();
            }
        }



    }
}