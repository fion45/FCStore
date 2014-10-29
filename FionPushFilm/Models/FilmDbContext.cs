using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.IO;

namespace FionPushFilm.Models
{
    public class FilmDbContext : DbContext
    {
        public ObjectContext m_objcontext;

        public FilmDbContext()
        {
            try
            {
                m_objcontext = ((IObjectContextAdapter)this).ObjectContext;
            }
            catch(Exception ex)
            {
                File.WriteAllText("D:\\FilmWeb\\web\\err.txt", ex.Message + this.Database.Connection.ConnectionString);
                m_objcontext = null;
            }
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<LoginPageTrail> LoginPageTrails { get; set; }

        public DbSet<ClientTrail> ClientTrails { get; set; }

        public DbSet<SearchLog> SearchLogs { get; set; }
    }
}