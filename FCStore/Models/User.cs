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

        public string LoginID
        {
            get;
            set;
        }

        [JsonIgnore]
        public string LoginPSW
        {
            get;
            set;
        }

        [ForeignKey("Role")]
        public int RID
        {
            get;
            set;
        }

        [JsonIgnore]
        public Role Role
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
    }
}