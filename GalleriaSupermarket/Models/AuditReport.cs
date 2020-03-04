using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models
{
    public class AuditReport
    {
        [Key]
        public int AuditReportID { get; set; }
        public string Description { get; set; }
    }
}