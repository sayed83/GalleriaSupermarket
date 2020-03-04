using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryID { get; set; }
        [Required]
        [Display(Name = "Subcategory")]
        public string SubCategoryName { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Size> Sizes { get; set; }
        public virtual ICollection<Brand> Brands { get; set; }
    }
}