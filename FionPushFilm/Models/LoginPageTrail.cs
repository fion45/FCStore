using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FionPushFilm.Models
{
    [Serializable]
    public class LoginPageTrail
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LPTID
        {
            get;
            set;
        }

        public int ErrorCount
        {
            get;
            set;
        }

        public string ClientIP
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