using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Brand
    {
        [Key]
        [Display(Name ="Brand")]
        public int BrandID { get; set; }
        [Display(Name ="Brand Name")]
        public string BrandName { get; set; }
        [Display(Name ="Availability")]
        public bool IsAvailable { get; set; }
        public int SubCategoryID { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}