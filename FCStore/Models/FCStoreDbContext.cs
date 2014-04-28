using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCStore.Models
{
    public class FCStoreDbContext : DbContext
    {
        public FCStoreDbContext() 
            //: base("name=FCStore")
        {
        }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categorys { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductTag> ProductTags { get; set; }

        public DbSet<Column> Columns { get; set; }
    }
}