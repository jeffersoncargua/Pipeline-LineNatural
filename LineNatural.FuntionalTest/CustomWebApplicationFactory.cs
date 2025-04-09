using LineNatural.Context;
using LineNatural.SharedDatabaseSetup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LineNatural.FuntionalTest
{
    //WebApplicationFactory -> se utiliza para crear una instancia de su aplicación web en la memoria
    //TStartup -> es un archivo en C# que se encarga de configurar los servicios y la canalización de solicitudes de una aplicación ASP.NET Core. 
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Elimina el registro ApplicationDbContext de la aplicación.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Añade ApplicationDbContext utilizando una base de datos en memoria para realizar pruebas.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForFunctionalTesting");
                });

                // Obtiene el proveedor de servicios
                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    var storeDbContext = scopedServices.GetRequiredService<ApplicationDbContext>();
                    storeDbContext.Database.EnsureCreated();

                    try
                    {
                        DatabaseSetup.SeedData(storeDbContext); //LLama al metodo SeedData() para agregar los datos simulados en memoria
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the Store database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }

        public void CustomConfigureServices(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Obtiene el proveedor de servicios
                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    var storeDbContext = scopedServices.GetRequiredService<ApplicationDbContext>();

                    try
                    {
                        DatabaseSetup.SeedData(storeDbContext); //LLama al metodo SeedData() para agregar los datos simulados en memoria
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the Store database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
