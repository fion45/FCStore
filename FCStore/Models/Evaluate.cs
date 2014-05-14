using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FCStore.Models
{
    public class Evaluate
    {
        public int EID
        {
            get;
            set;
        }

        public int UID
        {
            get;
            set;
        }

        public User User
        {
            get;
            set;
        }

        public Product Product
        {
            get;
            set;
        }

        public Order Order
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int StarCount
        {
            get;
            set;
        }
    }
}