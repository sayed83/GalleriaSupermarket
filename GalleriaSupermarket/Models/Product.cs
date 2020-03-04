using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        [Display(Name ="Product Name"), Required]
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        [Display(Name ="Other Price")]
        public decimal OtherPrice { get; set; }
        [DataType(DataType.Currency)]
        public decimal Discount { get; set; }
        [DataType(DataType.Currency)]
        public decimal Vat { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Net Price")]
        public decimal NetPrice { get { return ((Price + OtherPrice + Vat) - Discount); } }
        [Display(Name = "Subcategory")]
        public int SubCategoryID { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public int BrandID { get; set; }
        public virtual Brand Brand { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<ImageAlbum> ImageAlbumes { get; set; }
        public virtual ICollection<ProductSizeHeader> ProductSizeHeaders { get; set; }

    }
}