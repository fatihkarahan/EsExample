using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Models
{
    public class SearchParameters
    {
        public SearchParameters()
        {
            ColorSpecIds = new List<int>();
            MaterialSpecIds = new List<int>();
            UsedSpecIds = new List<int>();
        }

        private int _size = 24;
        private int _page = 1;
        private double _minPrice = -1;
        private double _maxPrice = -1;
        private string _categoryIds = "0";
        private string _sortfield = string.Empty;
        private string _sortorder = string.Empty;
        private string _q = string.Empty;
        private string _manufacturerIds = "0";
        private int _vendorId = 0;
        private bool _isEnableOffer = false;
        private bool _topRated = false;
        public string _shopbyIds = "0";
        public bool _isShowCloseProduct = false;

        public int SelectedCatId { get; set; }
        public int VendorId
        {
            get { return _vendorId; }
            set { _vendorId = value; }
        }
        public List<int> ColorSpecIds
        {
            get; set;
        }
        public List<int> MaterialSpecIds
        {
            get; set;
        }
        public List<int> UsedSpecIds
        {
            get; set;
        }
        public bool IsEnableOffer
        {
            get { return _isEnableOffer; }
            set { _isEnableOffer = value; }
        }
        public bool Toprated
        {
            get { return _topRated; }
            set { _topRated = value; }
        }
        public string ShopbyIds
        {
            get { return _shopbyIds; }
            set { _shopbyIds = value; }
        }
        public int Page
        {
            get { return _page; }
            set { _page = value; }
        }
        public int Size
        {
            get { return _size; }
            set
            {
                _size = 24;
            }
        }
        public string Q
        {
            get { return _q; }
            set { _q = value; }
        }
        public string ManufacturerIds
        {
            get { return _manufacturerIds; }
            set { _manufacturerIds = value; }
        }
        public string CategoryIds
        {
            get { return _categoryIds; }
            set { _categoryIds = value; }
        }
        public double MinPrice
        {
            get { return _minPrice; }
            set { _minPrice = value; }
        }
        public double MaxPrice
        {
            get { return _maxPrice; }
            set { _maxPrice = value; }
        }
        public string Sortfield
        {
            get { return _sortfield; }
            set { _sortfield = value; }
        }
        public string Sortorder
        {
            get { return _sortorder; }
            set { _sortorder = value; }
        }
        public bool IsShowCloseProduct
        {
            get { return _isShowCloseProduct; }
            set { _isShowCloseProduct = value; }
        }
    }
}
