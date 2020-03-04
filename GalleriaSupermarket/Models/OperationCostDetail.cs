using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class OperationCostDetail
    {
        [Key]
        public int OperationCostDetailID { get; set; }
        [Display(Name = "Cost Head"), Required]
        public string CostHead { get; set; }
        public int ExpenseID { get; set; }
        public virtual Expense Expense { get; set; }
        [Display(Name = "Cost Amount")]
        public decimal CostAmount { get; set; }
        [Display(Name = "Added By")]
        public string AddedBy { get; set; }
        [Display(Name = "Date")]
        public DateTime CostDate { get; set; }

    }
}