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
    public class CustomOrder
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int COID
        {
            get;
            set;
        }

        public string CountryName
        {
            get;
            set;
        }

        public string BrandName
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public decimal MinPrice
        {
            get;
            set;
        }

        public decimal MaxPrice
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }
    }
}