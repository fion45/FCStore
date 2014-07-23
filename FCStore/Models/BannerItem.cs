using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
    public class BannerItem
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BIID
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string ImgPath
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public string HrefPath
        {
            get;
            set;
        }
    }
}