using Bogus;
using LineNatural.Context;
using LineNatural.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineNatural.SharedDatabaseSetup
{
    public static class DatabaseSetup
    {
        
        public static void SeedData(ApplicationDbContext context)
        {
            ////Categories Fake with bogus
            
            //Se elimina los registros para que no se sobreescriban o copen la base de datos virtual y luego se guarda
            context.CategoryTbl.RemoveRange(context.CategoryTbl);
            context.SaveChanges(); 

            int categoryId = 1;
            var fakeCategories = new Faker<Category>()
               .RuleFor(o => o.CategoryName, f => $"Category {categoryId}")
               .RuleFor(o => o.Id, f => categoryId++);

            var categories = fakeCategories.Generate(1000); //Se genera los registros fake de Category para almacenar en la base de datos

            //Se almacenan los registros fake a la base de datos para realizar las pruebas
            context.CategoryTbl.AddRange(categories);
            context.SaveChanges();


            //Categories Fake with bogus
            //Se elimina los registros para que no se sobreescriban o copen la base de datos virtual y luego se guarda
            context.ProductoTbl.RemoveRange(context.ProductoTbl);
            context.SaveChanges(); 

            int productId = 1;
            var fakeProducts = new Faker<Producto>()
               .RuleFor(o => o.ProductName, f => $"Producto {productId}")
               .RuleFor(o => o.Description, f => $"Description {productId}")
               .RuleFor(o => o.Price, f => f.Random.Double(0.0, 999.99))
               .RuleFor(o => o.Stock, f => f.Random.Number(0, 100))
               .RuleFor(o => o.CategoryId, f => f.Random.Number(1, 1000))
               .RuleFor(o => o.DateCaducated, f => DateTime.Now)
               .RuleFor(o => o.Id, f => productId++);

            var products = fakeProducts.Generate(1000); //Se generan los registros fake de Productos

            //Se almacenan los regstros fake en la base de datos para realizar las pruebas
            context.ProductoTbl.AddRange(products);
            context.SaveChanges();


            //Fake Users with Bogus
            //context.Users.RemoveRange(context.ApplicationUserTbl);
            //context.SaveChanges();

            //int NameCount = 1;

            //var fakeUser = new Faker<ApplicationUser>()
            //    .RuleFor(o => o.Name, f => $"Name {NameCount}")
            //    .RuleFor(o => o.UserName, f => $"UserName{NameCount}@gmail.com")
            //    .RuleFor(o => o.PasswordHash, f => $"Admin123{NameCount++}!")
            //    .RuleFor(o => o.EmailConfirmed, f => true);

            //var users = fakeUser.Generate(1000);

            //context.ApplicationUserTbl.AddRange(users);
            //context.SaveChanges();

        }
    }
}
