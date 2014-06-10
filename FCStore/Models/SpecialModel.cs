using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCStore.Models
{
    [Serializable]
    public class SubmitObj
    {
        [Serializable]
        public class PacketObj
        {
            public int PacketID;
            public int Count;
        }
        public int OrderID
        {
            get;
            set;
        }

        public PacketObj[] Packets
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