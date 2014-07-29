using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FCStore.Models
{
    [Serializable]
    public class PushInfo
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PIID
        {
            get;
            set;
        }

        public string PushDT
        {
            get;
            set;
        }

        public int Weight
        {
            get;
            set;
        }

        [ForeignKey("Product")]
        public int PID
        {
            get;
            set;
        }

        public virtual Product Product
        {
            get;
            set;
        }

        [ForeignKey("Category")]
        public int? CID
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
        public int? BID
        {
            get;
            set;
        }

        public virtual Brand Brand
        {
            get;
            set;
        }

        public virtual List<ProductTag> Tags
        {
            get;
            set;
        }

        public virtual List<User> Users
        {
            get;
            set;
        }

    }
}