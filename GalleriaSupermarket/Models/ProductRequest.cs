using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class ProductRequest
    {
        [Key]
        public int ProductRequestID { get; set; }
        public int OutletId { get; set; }
        public virtual Outlet Outlet { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime RequestDate { get; set; }
    }
}