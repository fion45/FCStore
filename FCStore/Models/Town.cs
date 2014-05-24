using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace FCStore.Models
{
    public class Town
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TownID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public string PostCode3
        {
            get;
            set;
        }

        [ForeignKey("BelongCity")]
        public int BCID
        {
            get;
            set;
        }

        public virtual City BelongCity
        {
            get;
            set;
        }

        public string FullName
        {
            get
            {
                return BelongCity.BelongProvince.Name + " " + BelongCity.Name + " " + Name;
            }
        }
    }
}