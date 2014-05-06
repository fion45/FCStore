﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
    public class Column
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ColumnID
        {
            get;
            set;
        }

        public string Describe
        {
            get;
            set;
        }

        public virtual List<Product> Products
        {
            get;
            set;
        }
    }
}