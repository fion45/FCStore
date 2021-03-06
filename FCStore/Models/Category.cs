﻿using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FCStore.Models
{
    [Serializable]
    public class Category
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CID
        {
            get;
            set;
        }

        public int ParCID
        {
            get;
            set;
        }

        [NotMapped]
        [JsonIgnore]
        public virtual Category Parent
        {
            get;
            set;
        }

        public string NameStr
        {
            get;
            set;
        }

        public int Tag
        {
            get;
            set;
        }

        public bool IsShow
        {
            get;
            set;
        }
    }
}