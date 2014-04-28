using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
    public class Brand
    {
        [Key,DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BID
        {
            get;
            set;
        }

        public string NameStr
        {
            get;
            set;
        }

        public string Name2
        {
            get;
            set;
        }

        public int CountryCode
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