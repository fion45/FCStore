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
    public class ProductTag
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PTID
        {
            get;
            set;
        }

        public string Describe
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual List<Product> Products
        {
            get;
            set;
        }
    }
}