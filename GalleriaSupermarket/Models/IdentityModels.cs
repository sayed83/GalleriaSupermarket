using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GalleriaSupermarket.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string UserType { get; set; }
        public bool Agreement { get; set; }
        public string FullName { get; set; }
        public int? OutletID { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public HttpPostedFileBase UserImageUpload { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DbCon", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Category> Categories { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.SubCategory> SubCategories { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Purchase> Purchases { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.PurchaseInvoice> PurchaseInvoices { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Outlet> Outlets { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Supplier> Suppliers { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Size> Sizes { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.ProductSizeHeader> ProductSizeHeaders { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Product> Products { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.ImageAlbum> ImageAlbums { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Brand> Brands { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.SupplierPayment> SupplierPayments { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.AuditReport> AuditReports { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.LostProduct> LostProducts { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Item> Items { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.InternalSale> InternalSales { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.OnlineSale> OnlineSales { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.SalesReturn> SalesReturns { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Invoice> Invoices { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.ProductRequest> ProductRequests { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.DefectedProduct> DefectedProducts { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.ItemTransfer> ItemTransfers { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.OnlineInvoice> OnlineInvoices { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.ShippingAddress> ShippingAddresses { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.Expense> Expenses { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.OperationCostDetail> OperationCostDetails { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.City> Cities { get; set; }
        public System.Data.Entity.DbSet<GalleriaSupermarket.Models.PurchaseReturn> PurchaseReturns { get; set; }


    }
}