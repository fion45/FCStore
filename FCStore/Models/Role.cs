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
    public class Role
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RID
        {
            get;
            set;
        }

        public string RoleName
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Permission
        {
            get;
            set;
        }

        [JsonIgnore]
        public List<User> Users
        {
            get;
            set;
        }
    }
}