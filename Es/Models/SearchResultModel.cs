using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Models
{
    public class SearchResultModel
    {
        public SearchResultModel()
        {
            Manufacturers = new List<int>();
            Categories = new List<int>();
            ColorSpecs = new List<int>();
            MaterialSpecs = new List<int>();
        }
        public string SearchTerm { get; set; }
        public IList<ProductModel> Products { get; set; }
        public long HitCount { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Manufacturers { get; set; }
        public List<int> ColorSpecs { get; set; }
        public List<int> MaterialSpecs { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }

    }
}
