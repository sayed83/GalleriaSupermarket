using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class ProductImageVm
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public decimal OtherPrice { get; set; }
        [DataType(DataType.Currency)]
        public decimal Discount { get; set; }
        [DataType(DataType.Currency)]
        public decimal Vat { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public int SubCategoryID { get; set; }
        public string SubCategoryName { get; set; }
        public int ImageAlbumID { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
    }
}