using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
    public class ProductTag
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PTID
        {
            get;
            set;
        }

        public string Describe
        {
            get;
            set;
        }

        public List<Product> Products
        {
            get;
            set;
        }
    }
}