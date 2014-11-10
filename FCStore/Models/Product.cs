using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FCStore.Models
{
    [Serializable]
    public class Product
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PID
        {
            get;
            set;
        }

        [ForeignKey("Category")]
        public int CID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Category Category
        {
            get;
            set;
        }

        [ForeignKey("Brand")]
        public int BID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Brand Brand
        {
            get;
            set;
        }

        public virtual List<ProductTag> ProductTags
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }
        public string Chose
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public decimal MarketPrice
        {
            get;
            set;
        }

        public int Discount
        {
            get;
            set;
        }

        public int Stock
        {
            get;
            set;
        }

        public int Sale
        {
            get;
            set;
        }

        public string ImgPath
        {
            get;
            set;
        }

        public int PVCount
        {
            get;
            set;
        }

        [NotMapped]
        public string[] ImgPathArr
        {
            get
            {
                return this.ImgPath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string Descript
        {
            get;
            set;
        }

        public string Date
        {
            get;
            set;
        }

        public int Tag
        {
            get;
            set;
        }

        [NotMapped]
        public virtual List<ReColumnProduct> REProColLST
        {
            get;
            set;
        }

        [JsonIgnore]
        [NotMapped]
        public virtual List<Column> Columns
        {
            get
            {
                List<Column> result = new List<Column>();
                if (REProColLST != null)
                {
                    foreach (ReColumnProduct rcpItem in this.REProColLST)
                    {
                        result.Add(rcpItem.Column);
                    }
                }
                return result;
            }
        }

        public int EvaluationStarCount                         //虚假数据
        {
            get;
            set;
        }

        public virtual List<ShamOrderData> SaleCountLST       //虚假数据
        {
            get;
            set;
        }
    }

    //虚假订单类
    public class ShamOrderData
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SOID
        {
            get;
            set;
        }

        [ForeignKey("Product")]
        public int ProductID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Product Product
        {
            get;
            set;
        }

        public string IDLabel
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

        public string DateTime
        {
            get;
            set;
        }

    }
}