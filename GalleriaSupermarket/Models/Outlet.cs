using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Outlet
    {
        [Key]
        public int OutletID { get; set; }
        [Display(Name = "Name"), Required]
        public string OutletName { get; set; }
        [Display(Name = "Location Name"), Required]
        public string ShortAddress { get; set; }
        [Display(Name = "Details Address")]
        public string DetailsAddress { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime CreationDate { get; set; }
        public int CityID { get; set; }
        public virtual City City { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<ItemTransfer> ItemTransfers { get; set; }
        public virtual ICollection<LostProduct> LostProducts { get; set; }
    }
}