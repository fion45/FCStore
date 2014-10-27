using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace FionPushFilm.Models
{
    public class FilmDbContext : DbContext
    {
        public ObjectContext m_objcontext;

        public FilmDbContext()
        {
            m_objcontext = ((IObjectContextAdapter)this).ObjectContext;
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<LoginPageTrail> LoginPageTrails { get; set; }

        public DbSet<ClientTrail> ClientTrails { get; set; }

        public DbSet<SearchLog> SearchLogs { get; set; }
    }
}