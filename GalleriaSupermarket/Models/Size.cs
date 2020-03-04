using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Size
    {
        [Key]
        public int SizeID { get; set; }
        [Display(Name = "Size")]
        public string ItemSize { get; set; }
        [Display(Name = "Category")]
        public int SubCategoryID { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<ProductSizeHeader> ProductSizeHeaders { get; set; }
    }
}