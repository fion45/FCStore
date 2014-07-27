using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FCStore.Models
{
    public class RecentView
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RVID
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

        public User User
        {
            get;
            set;
        }

        [ForeignKey("Product")]
        public int PID
        {
            get;
            set;
        }

        public Product Product
        {
            get;
            set;
        }

        public string ViewDT
        {
            get;
            set;
        }
    }
}