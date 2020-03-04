using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GalleriaSupermarket.Models;
using System.IO;
using GalleriaSupermarket.Models.ViewModel;

namespace GalleriaSupermarket.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        public ActionResult Index()
        {
            List<ProductImageVm> pivm = new List<ProductImageVm>();
            var product = (from p in db.Products
                           join i in db.ImageAlbums
                           on p.ProductID equals i.ProductID
                           select new
                           {
                               id = p.ProductID,
                               name = p.ProductName,
                               imgid = i.ImageAlbumID,
                               image = i.ImageUrl,
                               punit = p.Unit,
                               price = p.Price,
                               otherPrice = p.OtherPrice,
                               discount = p.Discount,
                               vat = p.Vat,
                               sid = p.SubCategoryID,
                               subname = p.SubCategory.SubCategoryName,
                               category = p.SubCategory.Category.CategoryID,
                               catname = p.SubCategory.Category.CategoryName,
                               brandid = p.Brand.BrandID,
                               brand = p.Brand.BrandName
                           });

            
            foreach (var item in product)
            {
                ProductImageVm pimg = new ProductImageVm();
                pimg.ProductID = item.id;
                pimg.ImageAlbumID = item.imgid;
                pimg.ProductName = item.name;
                pimg.ImageUrl = item.image;
                pimg.Unit = item.punit;
                pimg.Price = item.price;
                pimg.OtherPrice = item.otherPrice;
                pimg.Discount = item.discount;
                pimg.Vat = item.vat;
                pimg.BrandID = item.brandid;
                pimg.BrandName = item.brand;
                pimg.SubCategoryID = item.sid;
                pimg.SubCategoryName = item.subname;
                pimg.CategoryName = item.catname;
                pimg.CategoryID = item.category;
                pivm.Add(pimg);
            }
            return View(pivm);
        }

        public JsonResult GetPriceBasedOnsubCategory(int id)
        {
            var saleingPrice = db.Purchases.Where(p => p.SubcategoryID == id).Select(p => new {
                price = p.Price,
                id
            = p.PurchaseID
            }).FirstOrDefault();
            if (saleingPrice != null)
            {
                return Json(saleingPrice, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult GetFruitSize(int id)
        {
            var fandvSize = db.Sizes.Where(s => s.SubCategoryID == id).AsEnumerable().Select(s => new { id = s.SizeID, size = s.ItemSize });
            return Json(fandvSize, JsonRequestBehavior.AllowGet);
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            ViewBag.SubCategoryID = new SelectList(db.SubCategories, "SubCategoryID", "SubCategoryName");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductSizeVm product)
        {
            int checkProduct = db.Products.Where(p => p.ProductName.Trim() == product.ProductName.Trim()).Count();
            if (checkProduct > 0)
            {
                ModelState.AddModelError("", "Product Already Exists!");
            }
            else
            {
                if (product.ImageUpload != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(product.ImageUpload.FileName);
                    string extension = Path.GetExtension(product.ImageUpload.FileName);
                    filename = filename + "_" + DateTime.Now.ToString("yymmssfff") + extension;

                    Product p = new Product();
                    p.ProductName = product.ProductName;
                    p.Unit = product.Unit;
                    p.Price = product.Price;
                    p.OtherPrice = product.OtherPrice;
                    p.Discount = product.Discount;
                    p.Vat = product.Vat;
                    p.BrandID = product.BrandID;
                    p.SubCategoryID = product.SubCategoryID;
                    p.IsDeleted = false;
                    db.Products.Add(p);
                    db.SaveChanges();

                    int lasProductId = p.ProductID;

                    ImageAlbum im = new ImageAlbum();
                    im.ProductID = lasProductId;
                    im.ImageUrl = filename;
                    db.ImageAlbums.Add(im);
                    db.SaveChanges();                    

                    for (var i = 0; i < product.SizeID.Count(); i++)
                    {
                        ProductSizeHeader psh = new ProductSizeHeader();
                        psh.ProductID = lasProductId;
                        psh.SizeID = product.SizeID[i];
                        db.ProductSizeHeaders.Add(psh);
                        db.SaveChanges();

                    }
                    product.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/App_File/ProductImage"), filename));
                    return RedirectToAction("Index");
                }
            }

            ViewBag.SubCategoryID = new SelectList(db.SubCategories, "SubCategoryID", "SubCategoryName", product.SubCategoryID);
            return View(product);
        }

        //Get Brands
        public JsonResult GetBrandbySubcategory(int brandId)
        {
            var brandList = db.Brands.Where(b=>b.SubCategoryID==brandId && b.IsDeleted == false && b.IsAvailable == true).AsEnumerable().Select(b => new { id = b.BrandID, name = b.BrandName });
            if(brandList != null)
            {
                return Json(brandList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = (from p in db.Products
                           join i in db.ImageAlbums
                           on p.ProductID equals i.ProductID
                           where i.ProductID == id
                           select new
                           {
                               id = p.ProductID,
                               name = p.ProductName,
                               imgid = i.ImageAlbumID,
                               image = i.ImageUrl,
                               punit = p.Unit,
                               price = p.Price,
                               otherPrice = p.OtherPrice,
                               discount = p.Discount,
                               vat = p.Vat,
                               sid = p.SubCategoryID,
                               subname=p.SubCategory.SubCategoryName,
                               category = p.SubCategory.Category.CategoryID,
                               catname = p.SubCategory.Category.CategoryName,
                               brandid=p.Brand.BrandID,
                               brand=p.Brand.BrandName
                           }).FirstOrDefault();

            ProductImageVm pimg = new ProductImageVm();
            pimg.ProductID = product.id;
            pimg.ImageAlbumID = product.imgid;
            pimg.ProductName = product.name;
            pimg.ImageUrl = product.image;
            pimg.Unit = product.punit;
            pimg.Price = product.price;
            pimg.OtherPrice = product.otherPrice;
            pimg.Discount = product.discount;
            pimg.Vat = product.vat;
            pimg.BrandID = product.brandid;
            pimg.BrandName = product.brand;
            pimg.SubCategoryID = product.sid;
            pimg.SubCategoryName = product.subname;
            pimg.CategoryName = product.catname;
            pimg.CategoryID = product.category;

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(pimg);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductImageVm product)
        {
            if (ModelState.IsValid)
            {
                var p = db.Products.Where(pr => pr.ProductID == product.ProductID).FirstOrDefault();
                p.ProductName = product.ProductName;
                p.Unit = product.Unit;
                p.SubCategoryID = product.SubCategoryID;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();

                var img = db.ImageAlbums.Where(i => i.ProductID == product.ProductID).FirstOrDefault();
                var oldImg = img.ImageUrl;
                string filename = Path.GetFileNameWithoutExtension(product.ImageUpload.FileName);
                string extension = Path.GetExtension(product.ImageUpload.FileName);
                filename = filename + "_" + DateTime.Now.ToString("yymmssfff") + extension;
                img.ImageUrl = filename;
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/App_File/ProductImage"), oldImg));
                product.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/App_File/ProductImage"), filename));
                img.ProductID = product.ProductID;
                db.Entry(img).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
