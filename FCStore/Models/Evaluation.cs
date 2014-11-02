using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FCStore.Models
{
    [Serializable]
    public class Evaluation
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EID
        {
            get;
            set;
        }

        [ForeignKey("User")]
        public int UID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual User User
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

        [JsonIgnore]
        public virtual Product Product
        {
            get;
            set;
        }

        [ForeignKey("Order")]
        public int OID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Order Order
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

        public bool IsShow
        {
            get;
            set;
        }
    }
}