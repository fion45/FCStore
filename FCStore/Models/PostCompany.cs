using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
    public class PostCompany
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PCID
        {
            get;
            set;
        }

        public string CompanyName
        {
            get;
            set;
        }

        public string HomePage
        {
            get;
            set;
        }
    }
}