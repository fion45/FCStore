using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FCStore.Models
{
    //记录用户浏览过的产品，用于用户管理页面的提示
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

        public virtual User User
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

        public virtual Product Product
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