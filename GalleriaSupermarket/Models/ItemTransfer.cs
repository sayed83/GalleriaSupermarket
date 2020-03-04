using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalleriaSupermarket.Models
{
    public class ItemTransfer
    {
        [Key]
        public int ItemTransferID { get; set; }
        public int OutletID { get; set; }
        public virtual Outlet Outlet { get; set; }
        public string Item { get; set; }
        public int ItemFrom { get; set; }
        public bool ManagerApproved { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }
    }
}