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
        public enum RoleTypeID
        {
            RT_ADMIN = 1,
            RT_FOREIGNSUPPLIER = 2,
            RT_SUPPLIER = 3,
            RT_SALE = 4,
            RT_CLIENT = 5,
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
        public List<ReUserRole> ReUserRoleLST
        {
            get;
            set;
        }
    }

    public class ReUserRole
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RURID
        {
            get;
            set;
        }

        [ForeignKey("User")]
        public int UID
        {
            get;
            set;
        }

        public virtual User User
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

        public virtual Role Role
        {
            get;
            set;
        }

        public string Reserve
        {
            get;
            set;
        }
    }
}