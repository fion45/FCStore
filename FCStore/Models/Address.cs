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

        public string _AddressName;

        public string AddressName
        {
            get
            {
                return _AddressName;
            }
            set
            {
                if ( BelongTown != null && (value.IndexOf(BelongTown.Name) == -1 || value.IndexOf(BelongTown.BelongCity.Name) == -1 ||
                    value.IndexOf(BelongTown.BelongCity.BelongProvince.Name) == -1))
                {
                    //不存在该地域
                    TownID = null;
                    BelongTown = null;
                }
                _AddressName = value;
            }
        }

        [NotMapped]
        public string FullAddress
        {
            get
            {
                return (TownID != null ? BelongTown.FullName + " " : "") + AddressName;
            }
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