﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalleriaSupermarket.Models.ViewModel
{
    public class CustomerVM
    {
        public string UserType { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
    }
}