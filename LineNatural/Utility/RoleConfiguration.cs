using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LineNatural.Utility
{
    /// <summary>
    /// Esta clase permite agregar informacion a IdentityRole para insertar roles en la base de datos
    /// cuando se lo cree, por tanto, si no se desea agregar estos datos en la base de datos real, se debe 
    /// quitar del archivo ApplicationDbContext la funcion overrride
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole()
                {
                    Name = "admin",
                    NormalizedName ="ADMIN"
                },
                new IdentityRole()
                {
                    Name = "customer",
                    NormalizedName = "Customer"
                });
        }
    }
}
