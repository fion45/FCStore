using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FCStore.Models
{
    [Serializable]
    public class Brand
    {
        [Key,DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BID
        {
            get;
            set;
        }

        public string NameStr
        {
            get;
            set;
        }

        public string Name2
        {
            get;
            set;
        }

        public string Img
        {
            get;
            set;
        }

        public int CountryCode
        {
            get;
            set;
        }

        public int Tag
        {
            get;
            set;
        }

        public int Important
        {
            get;
            set;
        }

        [JsonIgnore]
        [NotMapped]
        public virtual List<ReColumnBrand> REBrandColLST
        {
            get;
            set;
        }

        [JsonIgnore]
        [NotMapped]
        public virtual List<Column> Columns
        {
            get
            {
                List<Column> result = new List<Column>();
                foreach (ReColumnBrand rcpItem in this.REBrandColLST)
                {
                    result.Add(rcpItem.Column);
                }
                return result;
            }
        }
    }
}