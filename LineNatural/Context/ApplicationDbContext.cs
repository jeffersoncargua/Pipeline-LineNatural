using LineNatural.Entities;
using LineNatural.Utility;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LineNatural.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUserTbl { get; set; }
        public DbSet<Category> CategoryTbl { get; set; }
        public DbSet<Producto> ProductoTbl { get; set; }
        public DbSet<LocalUser> LocalUserTbl { get; set; }

        /// <summary>
        /// Esta funcion permite agregar informacion simulada de los roles en la base de datos simulada
        /// Cabe recalcar que se debe eliminar en caso de que se quiera realizar esta incorporacion 
        /// en la base de datos
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //RoleConfiguration contiene los informacion de los roles que vamos a simular
            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
