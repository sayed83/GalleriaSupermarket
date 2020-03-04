using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class ImageAlbum
    {
        [Key]
        public int ImageAlbumID { get; set; }
        public string ImageUrl { get; set; }
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }
    }
}