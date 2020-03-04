using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        [Display(Name = "Catagory Name")]
        [Required(ErrorMessage ="Category Name is Required")]
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}