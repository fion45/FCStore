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

        public string Contacts
        {
            get;
            set;
        }

        [ForeignKey("BelongTown")]
        public int? TownID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Town BelongTown
        {
            get;
            set;
        }

        public string AddressName
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public string PostCode
        {
            get;
            set;
        }
    }
}