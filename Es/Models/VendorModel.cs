using System;
using System.Collections.Generic;
using System.Text;
using Nest;

namespace Es.Models
{
   public class VendorModel
    {
        [Number(Name = "id", Index = true)]
        public int Id { get; set; }
        [Keyword(Name = "vendorname")]
        public string VendorName { get; set; }
        [Text(Name = "sename")]
        public string Sename { get; set; }
        [Text(Name = "imageurl")]
        public string ImageUrl { get; set; }

    }
}
