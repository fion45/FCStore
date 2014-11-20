using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.RegularExpressions;

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
            string tmpStr = "";
            if (tmpLen >= 3)
            {
                int tmpI = tmpLen / 3;
                tmpStr = eval.User.UserName.Substring(0, tmpI) + new string('*', tmpLen - 2 * tmpI) + eval.User.UserName.Substring(tmpLen - tmpI);
            }
            else
            {
                tmpStr = eval.User.UserName.Substring(0, 1) + "*";
            }
            this.IDLabel = string.Format("{0}({1})", tmpStr, eval.Order.BelongTown.FullName);
            this.Description = eval.Description;
            this.StarCount = eval.StarCount;
            this.DataTime = eval.DataTime;
            this.IsSham = false;
            this.IsShow = eval.IsShow;
        }

        public EvaluationVM(ShamOrderData shamOrder)
        {
            this.EID = shamOrder.SOID;
            this.IDLabel = shamOrder.IDLabel;
            this.Description = shamOrder.Description;
            this.DataTime = shamOrder.DateTime;
            this.StarCount = shamOrder.StarCount;
            this.IsSham = true;
            this.IsShow = true;
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

        public bool IsSham
        {
            get;
            set;
        }

        public bool IsShow
        {
            get;
            set;
        }
    }

    public class SaleLogVM
    {
        public List<int> CountArr
        {
            get;
            set;
        }

        public List<string> BDTStrArr
        {
            get;
            set;
        }

        public List<string> EDTStrArr
        {
            get;
            set;
        }

        public List<string> DTStrArr
        {
            get;
            set;
        }

        public List<int> ShamCountArr
        {
            get;
            set;
        }

        public List<int> ShamAddRealCountArr
        {
            get
            {
                List<int> resultLST = new List<int>();
                for(int i=0;i<ShamCountArr.Count;i++)
                {
                    resultLST.Add(ShamCountArr[i] + CountArr[i]);
                }
                return resultLST;
            }
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

    public class ManagerVM<T>
    {
        private List<T>     m_data;
        private Type        m_dtype;

        public ManagerVM(List<T> content, Dictionary<string, TableColumn.Config> cfgDic = null)
        {
            m_data = content;
            m_dtype = typeof(T);
            //获得所有列名
            TCArr = new List<TableColumn>();
            foreach (PropertyInfo pi in m_dtype.GetProperties())
            {
                if (cfgDic != null && (!cfgDic.ContainsKey(pi.Name) || cfgDic.ContainsKey(pi.Name) && cfgDic[pi.Name].ignore))
                    continue;
                TableColumn tmpTC = new TableColumn();
                tmpTC.Title = pi.Name;
                if (cfgDic != null && cfgDic.ContainsKey(pi.Name))
                {
                    TableColumn.Config tmpcfg = cfgDic[pi.Name];
                    tmpTC.Type = tmpcfg.specialType;
                    tmpTC.ClassName = tmpcfg.assLSTName;
                    tmpTC.Width = tmpcfg.width;
                    tmpTC.HtmlStr = tmpcfg.htmlStr;
                    tmpTC.Parameter = tmpcfg.parameter;
                }
                else
                {
                    tmpTC.Type = TableColumn.TCType.Text;
                    tmpTC.ClassName = "";
                    tmpTC.Width = 0;
                    tmpTC.HtmlStr = "";
                    tmpTC.Parameter = null;
                }
                TCArr.Add(tmpTC);
            }
            TIArr = new List<List<TableItem>>();
            int tmpIndex = 0;
            foreach(T obj in content)
            {
                List<TableItem> tiLST = new List<TableItem>();
                TIArr.Add(tiLST);
                foreach(TableColumn tmpTC in TCArr)
                {
                    TableItem tmpTI = new TableItem();
                    tmpTI.Description = m_dtype.InvokeMember(tmpTC.Title, BindingFlags.GetProperty, null, obj, null).ToString();
                    tmpTI.Key = m_dtype.InvokeMember(tmpTC.Title, BindingFlags.GetProperty, null, obj, null).ToString();
                    tmpTI.Item = obj;
                    TIArr[tmpIndex].Add(tmpTI);
                }
                ++tmpIndex;
            }
        }

        public class TableColumn
        {
            public enum TCType
            {
                ID,
                Text,
                MultiText,
                Selection,
                BoolTag,
                Img,
                Href
            }

            public class Config
            {
                public Config()
                {
                    specialType = TCType.Text;
                    assLSTName = "";
                    width = 0;
                    ignore = false;
                }

                public TCType specialType;
                public string assLSTName;
                public int width;
                public string htmlStr;
                public bool ignore;
                public object parameter;
            }
            public string Title;
            public TCType Type;
            public int Width;
            public string ClassName;
            public string HtmlStr;
            public object Parameter;
        }

        public class TableItem
        {
            public string Description;
            public object Key;
            public T Item;
        }

        public List<TableColumn> TCArr;

        public List<List<TableItem>> TIArr;

        private static Regex parRGX = new Regex("\\{(?'Property'.+)\\}");
        public static string ParseParameter(string par,TableItem item)
        {
            string result = par;
            Match tmpMatch = parRGX.Match(result);
            Type tmpType = item.Item.GetType();
            while (!string.IsNullOrEmpty(tmpMatch.Value))
            {
                string PropertyStr = tmpMatch.Groups["Property"].ToString();
                PropertyStr = tmpType.InvokeMember(PropertyStr, BindingFlags.GetProperty, null, item.Item, null).ToString();
                result = result.Substring(0, tmpMatch.Index) + PropertyStr + result.Substring(tmpMatch.Index + tmpMatch.Length);
                tmpMatch = parRGX.Match(result);
            }
            return result;
        }
    }

    public class ProductEditDetailVM
    {
        public Product Product;

        public List<EvaluationVM> EvaluationLST;

        public SaleLogVM SaleLog;
    }
}