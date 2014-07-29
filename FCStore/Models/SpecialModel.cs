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

    public class EvaluationVM
    {
        public EvaluationVM(Evaluation eval)
        {
            this.EID = eval.EID;
            int tmpLen = eval.User.UserName.Length;
            int tmpI = tmpLen / 3;
            string tmpStr = eval.User.UserName.Substring(0, tmpI) + new string('*', tmpLen - 2 * tmpI) + eval.User.UserName.Substring(tmpLen - tmpI);
            this.IDLabel = string.Format("{0}({1})", tmpStr, eval.Order.BelongTown.FullName);
            this.Description = eval.Description;
            this.StarCount = eval.StarCount;
            this.DataTime = eval.DataTime;
        }

        public int EID
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

        public string DataTime
        {
            get;
            set;
        }

        public string IDLabel
        {
            set;
            get;
        }
    }

    public class SaleLogVM
    {
        public List<int> CountArr
        {
            get;
            set;
        }

        public List<string> DTStrArr
        {
            get;
            set;
        }
    }

     public class UserDetailsVM
     {
         public User User
         {
             get;
             set;
         }

         public List<RecentView> RecentViewArr
         {
             get;
             set;
         }

         public List<PushInfo> PushInfoArr
         {
             get;
             set;
         }

         public List<Order> OrderArr
         {
             get;
             set;
         }
     }
}