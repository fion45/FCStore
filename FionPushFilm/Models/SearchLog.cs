using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FionPushFilm.Models
{
    [Serializable]
    public class SearchLog
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SLID
        {
            get;
            set;
        }

        public string IPAddress
        {
            get;
            set;
        }

        public string SearchStr
        {
            get;
            set;
        }

        public string LogDateTime
        {
            get;
            set;
        }
    }
}