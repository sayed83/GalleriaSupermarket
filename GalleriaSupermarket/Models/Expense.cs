using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseID { get; set; }
        [Display(Name = "Electricity Bill")]
        public decimal ElectricityBill { get; set; }
        [Display(Name = "Operation Cost")]
        public decimal OutletOperationCost { get; set; }
        public int OutletID { get; set; }
        public virtual Outlet Outlet { get; set; }
        [Display(Name = "Outlet Staff Salary")]
        public decimal OutletStaffSalary { get; set; }
        [Display(Name = "Other Expense")]
        public decimal OtherExpense { get; set; }
        [Display(Name = "Date")]
        public DateTime ExpenseIssueDate { get; set; }
        public decimal CashOnHand { get; set; }
        public string Month { get; set; }
        public bool IsReceived { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<OperationCostDetail> OperationCostDetails { get; set; }


    }
}