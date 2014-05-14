using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FCStore.Models
{
    [Serializable]
    public class Address
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AddID
        {
            get;
            set;
        }

        [ForeignKey("Area")]
        public int AID
        {
            get;
            set;
        }

        [JsonIgnore]
        public Area Area
        {
            get;
            set;
        }

        public string AddressName
        {
            get;
            set;
        }

        public string FullAddress
        {
            get
            {
                return "";
            }
        }

        public string Phone
        {
            get;
            set;
        }
    }
}