using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
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
    }
}