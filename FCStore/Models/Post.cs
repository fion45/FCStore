using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
    public class Post
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int POID
        {
            get;
            set;
        }

        [ForeignKey("PostCompany")]
        public int PCID
        {
            get;
            set;
        }

        public virtual PostCompany PostCompany
        {
            get;
            set;
        }

        public string PostOrderID
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

        public string LastUpdateDT
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}