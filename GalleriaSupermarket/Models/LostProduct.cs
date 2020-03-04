using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class LostProduct
    {
        [Key]
        public int LostProductID { get; set; }
        public int ItemID { get; set; }
        public virtual Item Item { get; set; }
        public int OutletID { get; set; }
        public virtual Outlet Outlet { get; set; }
        public int LostQnty { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy {get;set;}
        public bool IsDeleted { get; set; }
    }
}