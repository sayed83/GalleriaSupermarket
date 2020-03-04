using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class ProductSizeHeader
    {
        [Key]
        public int ProductSizeHeaderID { get; set; }
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
        public int SizeID { get; set; }
        public virtual Size Size { get; set; }
        public bool IsAvailable { get; set; }
    }
}