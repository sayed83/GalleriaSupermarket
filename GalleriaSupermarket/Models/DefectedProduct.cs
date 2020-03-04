using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class DefectedProduct
    {
        [Key]
        public int DefectedProductID { get; set; }
        public int ItemID { get; set; }
        public virtual Item Item { get; set; }
        [Display(Name = "Reason"), Required]
        public string DefectedReason { get; set; }
        [Display(Name = "Date")]
        public DateTime AddedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}