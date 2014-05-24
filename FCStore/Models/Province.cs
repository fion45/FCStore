using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace FCStore.Models
{
    public class Province
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProvinceID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string PostCode1
        {
            get;
            set;
        }

        public virtual List<City> CityArr
        {
            get;
            set;
        }
    }
}