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
    public class User
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UID
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string LoginID
        {
            get;
            set;
        }

        [DataType(DataType.Password)]
        [JsonIgnore]
        public string LoginPSW
        {
            get;
            set;
        }

        public virtual List<Role> Roles
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public virtual List<Address> Addresses
        {
            get;
            set;
        }

        public string Permission
        {
            get;
            set;
        }

        public uint Gift
        {
            get;
            set;
        }

        [NotMapped]
        public string Phone
        {
            get
            {
                return (Addresses != null && Addresses.Count > 0) ? Addresses[0].Phone : "";
            }
        }

        [NotMapped]
        public string[] PermissionTag
        {
            get
            {
                return null;
            }
        }
    }
}