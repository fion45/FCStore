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

        public Category Category
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

        public Brand Brand
        {
            get;
            set;
        }

        public List<ProductTag> ProductTags
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

        public List<Column> Columns
        {
            get;
            set;
        }
    }
}