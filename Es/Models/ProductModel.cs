using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Nest;

namespace Es.Models
{
    public class ProductModel : ISearchableModel
    {
        [Number(DocValues = true, Index = true)]
        public int Id { get; set; }
        [Text(Name = "name")]
        public string Name { get; set; }
        [Text(Name = "url")]
        public string SeName { get; set; }
        [Text(Name = "imageurl")]
        public string ImageUrl { get; set; }
        [Number(Name = "price")]
        public decimal Price { get; set; }
        [Number(Name = "oldprice")]
        public decimal OldPrice { get; set; }
        [Text(Name = "sku")]
        public string Sku { get; set; }
        [Keyword(Name = "manufacturer", Index = true)]
        public string ManufacturerName { get; set; }
        [Number(Name = "manufacturerid", Index = true)]
        public int ManufacturerId { get; set; }
        [Date()]
        public DateTime CreatedDate { get; set; }
        [PropertyName("categories"), Nested]
        public CategoriesModel Categories { get; set; }
        [PropertyName("vendor")]
        public VendorModel Vendor { get; set; }
        [Number(DocValues = false, Index = true, Name = "wishlistcount")]
        public int WhislistCount { get; set; }
        [Text(Name = "usedspec")]
        public string UsedSpec { get; set; }
        [Text(Name = "dimensionspec")]
        public string Dimensions { get; set; }
        [Number(DocValues = false, Index = true, Name = "reviewcount")]
        public int ReviewCount { get; set; }
        [Boolean(Name = "markasnew")]
        public bool MarkAsNew { get; set; }
        [PropertyName("colorspecs"), Nested]
        public SpecsModel ColorSpecs { get; set; }
        [PropertyName("materialspecs"), Nested]
        public SpecsModel MaterialSpecs { get; set; }
        [Boolean(Name = "isopensale")]
        public bool IsOpenSale { get; set; }
        [Number(Name = "productstatusid")]
        public int ProductStatusId { get; set; }
    }

}
