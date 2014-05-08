using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace FCStore.Models
{
    [Serializable]
    public class ProductListVM
    {
        public List<Product> Products;

        public List<Brand> Brands;

        public Category Category;

        public Brand Brand;

        public int PageCount;
        public int PageIndex;

        public bool IsFirst()
        {
            return PageIndex == 1;
        }

        public bool IsLast()
        {
            return PageIndex == PageCount;
        }
    }
}