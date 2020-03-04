using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using GalleriaSupermarket.Models;

namespace GalleriaSupermarket.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Roles
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            //Populate DropDown List
            var context = new ApplicationDbContext();
            var roleList = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.Roles = roleList;

            var outletList = context.Outlets.OrderBy(o => o.OutletName).ToList().Select(ou => new SelectListItem { Value = ou.OutletID.ToString(), Text = ou.OutletName });

            ViewBag.OutletID = outletList;

            var userList = context.Users.OrderBy(u => u.UserName).ToList().Select(uu => new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();

            ViewBag.Users = userList;
            ViewBag.Message = "";
            return View();
        }

        public JsonResult GetUserList()
        {
            var userList = db.Users.Where(u=>u.UserType != "Customer").AsEnumerable().Select(u => new { id = u.Id, name = u.UserName });
            if(userList != null)
            {
                return Json(userList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string RoleName)
        {
            var context = new ApplicationDbContext();
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index", "Role");
        }

        //Edit Get
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string RoleName)
        {
            var context = new ApplicationDbContext();
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            return View(thisRole);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(IdentityRole role)
        {
            try
            {
                var context = new ApplicationDbContext();
                context.Entry(role).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index", "Role");
            }
            catch
            {
                return View();
            }
        }

        //Create POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult Create(string RoleName)
        {
            bool status = false;
            string roleMaxID = db.Roles.Max(r => r.Id);
            int roleID = (Convert.ToInt32(roleMaxID)+1);
            if (!String.IsNullOrEmpty(RoleName))
            {
                IdentityRole role = new IdentityRole();
                role.Id = roleID.ToString();
                role.Name = RoleName;
                db.Roles.Add(role);
                db.SaveChanges();
                return Json(status = true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(status, JsonRequestBehavior.AllowGet);
            }

        }

        //GET User Roles
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!String.IsNullOrWhiteSpace(UserName))
            {
                var user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase) && u.UserType != "Customer").FirstOrDefault();

                
            }

            return View("Index");
        }

        //Adding Roles to a user
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName, int OutletID)
        {
            var context = new ApplicationDbContext();
            if (context == null)
            {
                throw new ArgumentException("context", "Context must not be null");
            }
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManger = new UserManager<ApplicationUser>(userStore);
            userManger.AddToRole(user.Id, RoleName);

            user.UserType = RoleName;
            user.OutletID = OutletID;
            context.Entry(user).State = EntityState.Modified;
            context.SaveChanges();

            ViewBag.Message = "Role Assigned Successfully";

            //Populate DropDown List
            var roleList = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.Roles = roleList;

            var outletList = context.Outlets.OrderBy(o => o.OutletName).ToList().Select(ou => new SelectListItem { Value = ou.OutletID.ToString(), Text = ou.OutletName });

            ViewBag.OutletID = outletList;
            

            var userList = context.Users.OrderBy(u => u.UserName).ToList().Select(uu => new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();

            ViewBag.Users = userList;

            return View("Index");

        }

        //Deleting a User From a Roles
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            var account = new AccountController();
            var context = new ApplicationDbContext();
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManger = new UserManager<ApplicationUser>(userStore);

            if (userManger.IsInRole(user.Id, RoleName))
            {
                userManger.RemoveFromRole(user.Id, RoleName);
                ViewBag.Message = "Role Removed from this user Successfully";
            }
            else
            {
                ViewBag.Message = "This user does't belgong to selected role!!!";
            }
            //Populate DropDown List
            var roleList = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.Roles = roleList;

            var userList = context.Users.OrderBy(u => u.UserName).ToList().Select(uu => new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();

            ViewBag.Users = userList;

            return View("Index", "Role");


        }
    }
}