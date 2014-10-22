using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FionPushFilm.Models
{
    [Serializable]
    public class Role
    {
        public enum RoleTypeID
        {
            RT_ADMIN = 1,
            RT_CLIENT = 2
        }

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