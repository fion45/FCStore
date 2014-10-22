using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FionPushFilm.Models
{
    [Serializable]
    public class ClientTrail
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CTID
        {
            get;
            set;
        }

        public int? UID
        {
            get;
            set;
        }

        public string ClientIP
        {
            get;
            set;
        }

        public string URL
        {
            get;
            set;
        }

        public string ControllerName
        {
            get;
            set;
        }

        public string ActionName
        {
            get;
            set;
        }

        public string LogDate
        {
            get;
            set;
        }
    }
}