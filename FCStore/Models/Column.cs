using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FCStore.Models
{
    public class Column
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ColumnID
        {
            get;
            set;
        }

        public string Describe
        {
            get;
            set;
        }

        public string SubDescribe
        {
            get;
            set;
        }

        public string TopTitle
        {
            get;
            set;
        }

        public string[] TopTitleArr
        {
            get
            {
                return TopTitle.Split(new char[] { ',' });
            }
        }

        [NotMapped]
        public virtual List<ReColumnProduct> REColProLST
        {
            get;
            set;
        }

        [NotMapped]
        [JsonIgnore]
        public virtual List<Product> Products
        {
            get
            {
                List<Product> result = new List<Product>();
                foreach(ReColumnProduct rcpItem in this.REColProLST)
                {
                    result.Add(rcpItem.Product);
                }
                return result;
            }
        }

        [NotMapped]
        public virtual List<ReColumnBrand> REColBrandLST
        {
            get;
            set;
        }

        [NotMapped]
        [JsonIgnore]
        public virtual List<Brand> Brands
        {
            get
            {
                List<Brand> result = new List<Brand>();
                foreach (ReColumnBrand rcbItem in this.REColBrandLST)
                {
                    result.Add(rcbItem.Brand);
                }
                return result;
            }
        }
    }

    public class ReColumnProduct
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RCPID
        {
            get;
            set;
        }

        [ForeignKey("Column")]
        public int ColumnID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Column Column
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

        public int CrossRow
        {
            get;
            set;
        }

        public int CrossColum
        {
            get;
            set;
        }

        public int RenderType
        {
            get;
            set;
        }
    }


    public class ReColumnBrand
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RCBID
        {
            get;
            set;
        }

        [ForeignKey("Column")]
        public int ColumnID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Column Column
        {
            get;
            set;
        }

        [ForeignKey("Brand")]
        public int BrandID
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual Brand Brand
        {
            get;
            set;
        }
    }
}