using GalleriaSupermarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using GalleriaSupermarket.Models.ViewModel;

namespace GalleriaSupermarket.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public HomeController()
        {
                
        }
        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            var categoryList = db.Categories.ToList();
            ViewBag.CategoryList = categoryList;
            return View();
        }
        public ActionResult CheckOut()
        {
            return View();
        }

        public ActionResult Thankyou()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult GetCustomer ()
        {
            List<CustomerVM> cvm = new List<CustomerVM>();
            var customerList = db.Users.Where(u => u.UserType == "Customer");
            foreach (var item in customerList)
            {
                CustomerVM obj = new CustomerVM();
                obj.FullName = item.FullName;
                obj.Email = item.Email;
                obj.UserType = item.UserType;
                obj.ImageUrl = item.ImageUrl;
                obj.UserName = item.UserName;
                cvm.Add(obj);
            }
            return View(cvm);

        }
        public ActionResult AuthenticationForCheckout()
        {
            return View();
        }
        public ActionResult Payments()
        {
            return View();
        }
        public JsonResult GetCategories()
        {
            var categoryList = db.Categories.AsEnumerable().Select(c => new { id = c.CategoryID, name = c.CategoryName });
            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBrandForPurchase()
        {
            var brandList = db.Brands.AsEnumerable().Select(b => new { id = b.BrandID, name = b.BrandName });
            return Json(brandList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNotifications()
        {
            int sid = Convert.ToInt32(Session["OutletID"].ToString());
            var notification = from s in db.Items
                               join p in db.Products
                               on s.ProductID equals p.ProductID
                               where s.AvailableQnty <= 5 && s.OutletID == sid
                               select new
                               {
                                   id = s.ItemID,
                                   name = p.ProductName,
                                   qnty = s.AvailableQnty

                               };
            return Json(notification, JsonRequestBehavior.AllowGet);
        }
        public JsonResult NotficationCount()
        {
            if(Session["OutletID"] != null)
            {
                int sid = Convert.ToInt32(Session["OutletID"].ToString());
                int count = db.Items.Where(s => s.AvailableQnty <= 5 && s.OutletID == sid).Count();
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            
        }

        //Get Product Request notification only see Admin
        public JsonResult ReqNotficationCount()
        {
            if (Session["OutletID"] != null)
            {
                int count = db.ProductRequests.Where(s => s.Status == false).Count();
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetProductReqNotifications()
        {
            var notification = from s in db.ProductRequests
                               where s.Status == false
                               select new
                               {
                                   id = s.ProductRequestID,
                                   name = s.Outlet.OutletName,
                               };
            if(notification != null)
            {
                return Json(notification, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetBrandBySubcategory(int subid)
        {
            var BrandList = db.Brands.Where(b => b.SubCategoryID == subid).AsEnumerable().Select(b => new { id = b.BrandID, name = b.BrandName });
            if(BrandList != null)
            {
                return Json(BrandList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetSubCategories(int id)
        {
            var subCategories = db.SubCategories.Where(s => s.CategoryID == id).AsEnumerable().Select(s => new { id = s.SubCategoryID, name = s.SubCategoryName });
            return Json(subCategories, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProducts(int id)
        {
            var products = db.Products.Where(s => s.SubCategoryID == id).AsEnumerable().Select(s => new { id = s.ProductID, name = s.ProductName });
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCity() {
            var cityList = db.Cities.AsEnumerable().Select(c => new { id = c.CityID, name = c.CityName });
            if(cityList != null)
            {
                return Json(cityList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UserAuthenticate()
        {
            if (Request.IsAuthenticated)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetOutletLocation(int cid)
        {
            var outletList = db.Outlets.Where(c=>c.CityID==cid && c.OutletID  != 1).AsEnumerable().Select(r => new { id = r.OutletID, name = r.OutletName, location = r.ShortAddress });
            return Json(outletList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemsByCategory()
        {
            var itemsList = from i in db.Items
                            join p in db.Products
                            on i.ProductID equals p.ProductID
                            join im in db.ImageAlbums
                            on p.ProductID equals im.ProductID
                            where i.OutletID == 1 && i.AvailableQnty >= 5
                            select new
                            {
                                id = i.ItemID,
                                proid = p.ProductID,
                                outletid = i.OutletID,
                                imageId=im.ImageAlbumID,
                                price = ((p.Price+p.OtherPrice+p.Vat)-p.Discount),
                                name = (i.Product.ProductName).Substring(0,20)+" ..",
                                image = im.ImageUrl
                            };
            return Json(itemsList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemsByOutlet(int? id)
        {
            var itemsList = from i in db.Items
                            join p in db.Products
                            on i.ProductID equals p.ProductID
                            join im in db.ImageAlbums
                            on p.ProductID equals im.ProductID
                            join o in db.Outlets
                            on i.OutletID equals o.OutletID
                            where i.OutletID == id && i.AvailableQnty >= 5
                            select new
                            {
                                id = i.ItemID,
                                proid=p.ProductID,
                                outletid = o.OutletID,
                                price = ((p.Price + p.OtherPrice + p.Vat) - p.Discount),
                                name = (i.Product.ProductName).Substring(0, 20) + " ..",
                                image = im.ImageUrl,
                                imageId = im.ImageAlbumID
                            };

            return Json(itemsList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSize(int? prosid)
        {

            var sizeList = (from psh in db.ProductSizeHeaders
                            where psh.ProductID == prosid
                            select new
                            {
                                id = psh.SizeID,
                                size = psh.Size.ItemSize
                            });
            if (sizeList != null)
            {
                return Json(sizeList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetItems(int bid)
        {
            var BrandItems = (
                                from p in db.Products join b in db.Brands on p.BrandID equals b.BrandID
                                join im in db.ImageAlbums on p.ProductID equals im.ProductID
                                where p.BrandID == bid
                                select new
                                {
                                    id = p.ProductID,
                                    brand = b.BrandName,
                                    price = ((p.Price + p.OtherPrice + p.Vat) - p.Discount),
                                    name = (p.ProductName).Substring(0, 20) + " ..",
                                    image = im.ImageUrl
                                }
                              );
            if(BrandItems != null)
            {
                return Json(BrandItems, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        
       
        public async Task<JsonResult> CheckUserAccount(string userName, string pass)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    ViewBag.errorMessage = "You must have a confirmed email to log on.";
                    return Json("invalid", JsonRequestBehavior.AllowGet);
                }
            }
            var result = await SignInManager.PasswordSignInAsync(userName, pass, false, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Json(true, JsonRequestBehavior.AllowGet);
                default:
                    return Json(false, JsonRequestBehavior.AllowGet);
            }


        }
        public string HashPass(string password)
        {

            byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("SHA1")).ComputeHash(encodedPassword);
            string encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

            return encoded;//returns hashed version of password
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}