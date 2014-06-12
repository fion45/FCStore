using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FCStore.Models
{
    public class PacketObj
    {
        public int PacketID
        {
            get;
            set;
        }
        public int Count
        {
            get;
            set;
        }
    }

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

    public class OrderVM
    {
        public List<Order> OrderArr
        {
            get;
            set;
        }
        public User Client
        {
            get;
            set;
        }
    }
}