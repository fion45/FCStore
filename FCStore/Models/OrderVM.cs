using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCStore.Models
{
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