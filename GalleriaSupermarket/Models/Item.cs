using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Item
    {
        [Key]
        public int ItemID { get; set; }
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        public int OutletID { get; set; }
        public virtual Outlet Outlet { get; set; }
        [Display(Name = "Quantity")]
        public int AvailableQnty { get; set; }
        [Display(Name = "In Stock?")]
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<InternalSale> InternalSales { get; set; }
        public virtual ICollection<OnlineSale> OnlineSales { get; set; }
        public virtual ICollection<DefectedProduct> DefectedProducts { get; set; }
        public virtual ICollection<LostProduct> LostProducts { get; set; }
    }
}