using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
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

        public virtual List<Column> Columns
        {
            get;
            set;
        }
    }
}