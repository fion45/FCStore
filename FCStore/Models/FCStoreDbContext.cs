using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace FCStore.Models
{
    public class FCStoreDbContext : DbContext
    {
        public ObjectContext m_objcontext;

        public FCStoreDbContext() 
            //: base("name=FCStore")
        {
            m_objcontext = ((IObjectContextAdapter)this).ObjectContext;
        }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categorys { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductTag> ProductTags { get; set; }

        public DbSet<Column> Columns { get; set; }

        public DbSet<Province> Province { get; set; }

        public DbSet<City> City { get; set; }

        public DbSet<Town> Town { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderPacket> OrderPackets { get; set; }

        public DbSet<Keep> Keeps { get; set; }

        public DbSet<Evaluation> Evaluations { get; set; }

        public DbSet<ClientTrail> ClientTrails { get; set; }

        public DbSet<LoginPageTrail> LoginPageTrails { get; set; }

        public DbSet<BannerItem> BannerItems { get; set; }

        public DbSet<RecentView> RecentViews { get; set; }

        public DbSet<PushInfo> PushInfos { get; set; }
    }
}