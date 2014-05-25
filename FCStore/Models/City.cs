using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace FCStore.Models
{
    public class City
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CityID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string PostCode2
        {
            get;
            set;
        }

        [ForeignKey("BelongProvince")]
        public int BPID
        {
            get;
            set;
        }

        public virtual Province BelongProvince
        {
            get;
            set;
        }

        public virtual List<Town> TownArr
        {
            get;
            set;
        }
    }
}