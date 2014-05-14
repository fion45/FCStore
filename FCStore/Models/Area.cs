using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FCStore.Models
{
    [Serializable]
    public class Area
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AID
        {
            get;
            set;
        }

        public int Countrycode
        {
            get;
            set;
        }

        public string CountryName
        {
            get;
            set;
        }

        public string ProvinceName
        {
            get;
            set;
        }

        public string CityName
        {
            get;
            set;
        }

        public string CountyName
        {
            get;
            set;
        }

        public int Postcode
        {
            get;
            set;
        }
    }
}