using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FCStore.Models
{
    public class Store
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int STID
        {
            get;
            set;
        }

        public string StoreName
        {
            get;
            set;
        }

        public List<ReStoreUser> REStoreUserLST
        {
            get;
            set;
        }

        public List<ReStoreProduct> REStoreProductLST
        {
            get;
            set;
        }
    }

    public class ReStoreUser
    {
        public enum AbilityType
        {
            AT_SALE = 0x01,             //销售能力
            AT_BARGAIN = 0x02,          //砍价能力
            AT_EDIT = 0x04,             //编辑能力
            AT_CHECK = 0x08,            //查看销售
            AT_ALL = AT_SALE | AT_BARGAIN | AT_EDIT | AT_CHECK 
        }

        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RSUID
        {
            get;
            set;
        }

        [ForeignKey("Store")]
        public int StoreID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Store Store
        {
            get;
            set;
        }

        [ForeignKey("User")]
        public int UserID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual User User
        {
            get;
            set;
        }

        public int Ability
        {
            get;
            set;
        }
    }

    public class ReStoreProduct
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RSPID
        {
            get;
            set;
        }

        [ForeignKey("Store")]
        public int StoreID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Store Store
        {
            get;
            set;
        }

        [ForeignKey("Product")]
        public int ProductID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Product Product
        {
            get;
            set;
        }
    }
}