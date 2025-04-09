using LineNatural.Entities;
using LineNatural.Repository;
using Microsoft.EntityFrameworkCore;

namespace LineNatural.IntegratedTest.Repository
{
    [Collection("My Collection")]
    //public class ProductoRepositoryTest : IClassFixture<SharedDatabaseFixture>
    public class ProductoRepositoryTest 
    {
        private SharedDatabaseFixture Fixture { get; }
        public ProductoRepositoryTest(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_ReturnAllProducts()
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new ProductRepository(context);

                //Act
                var response = await repository.GetAllAsync();

                //Assert
                Assert.Equal(1000, response.Count);
                Assert.IsType<List<Producto>>(response);
            }
        }

        [Theory]
        [InlineData("Producto 1")]
        [InlineData("Producto 10")]
        [InlineData("Producto 100")]
        [InlineData("Producto 1000")]
        public async Task GetAllAsync_ReturnAllProductsWithFilter(string search)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new ProductRepository(context);

                //Act
                var response = await repository.GetAllAsync(u => u.ProductName.Contains(search));

                //Assert
                Assert.Contains(search, response.First().ProductName);
            }
        }


        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task GetAsync_ReturnProductById(int id)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new ProductRepository(context);

                //Act
                var response = await repository.GetAsync(u => u.Id == id,includeProperties:"Category");

                //Assert
                Assert.Equal(id, response.Id);
            }
        }

        [Theory]
        [InlineData(1,true)]
        [InlineData(10,false)]
        [InlineData(100,true)]
        [InlineData(1000,false)]
        public async Task GetAsync_ReturnProductByIdAndTracked(int id,bool tracked)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new ProductRepository(context);

                //Act
                var response = await repository.GetAsync(u => u.Id == id, tracked ,includeProperties: "Category");

                //Assert
                Assert.Equal(id, response.Id);
            }
        }


        [Theory]
        [InlineData("Producto 1")]
        [InlineData("Producto 10")]
        [InlineData("Producto 100")]
        [InlineData("Producto 1000")]
        public async Task GetAsync_ReturnProductByFilter(string search)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new ProductRepository(context);

                //Act
                var response = await repository.GetAsync(u => u.ProductName.Contains(search), includeProperties: "Category");

                //Assert
                Assert.Contains(search,response.ProductName);
            }
        }


        [Theory]
        [InlineData(100000)]
        [InlineData(100001)]
        [InlineData(1000001)]
        public async Task GetAsync_ReturnProductFail(int id)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new ProductRepository(context);

                //Act
                var response = await repository.GetAsync(u => u.Id == id);

                //Assert
                Assert.Null(response);
            }
        }


        [Theory]
        [InlineData(1,"Producto 1001","Description 1001",989.99,2)]
        [InlineData(10, "Producto 1002", "Description 1002", 984.99, 0)]
        [InlineData(100, "Producto 1003", "Description 1003", 969.99, 1)]
        [InlineData(1000, "Producto 1004", "Description 1004", 929.99, 3)]
        public async Task CreateAsync_SavedCorrectProduct(int categoryId, string productName, string description, double price,int stock)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                //Arrange
                Producto producto = new()
                {
                    ProductName = productName,
                    Description = description,
                    Price = price,
                    Stock = stock,
                    CategoryId = categoryId,
                    DateCaducated = DateTime.Now                    
                };

                using (var context = Fixture.CreateContext(transaction)) 
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    await repository.CreateAsync(producto);

                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    var response = await repository.GetAsync(u => u.ProductName == productName && u.CategoryId == categoryId, includeProperties:"Category");

                    //Assert 
                    Assert.NotNull(response);

                }
            }
        }


        [Theory]
        [InlineData(1001, "Producto 1001", "Description 1001", 989.99, 2)]
        [InlineData(10, null, "Description 1002", 984.99, 1)]
        [InlineData(100, "Producto 1003", null, 969.99, 1)]
        public async Task CreateAsync_FailedSaveProduct(int categoryId, string productName, string description, double price, int stock)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                //Arrange
                Producto producto = new()
                {
                    ProductName = productName,
                    Description = description,
                    Price = price,
                    Stock = stock,
                    CategoryId = categoryId,
                    DateCaducated = DateTime.Now
                };

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    var action = async() => await repository.CreateAsync(producto);

                    //Assert
                    await Assert.ThrowsAsync<DbUpdateException>(action);

                }

            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task RemoveAsync_DeletedCorrectProduct(int id)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Producto producto;

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    producto = await repository.GetAsync(u => u.Id == id);
                    await repository.RemoveAsync(producto);

                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    var response = await repository.GetAsync(u => u.Id == id);

                    //Assert
                    Assert.Null(response);

                }

            }
        }

        [Theory]
        [InlineData(1001)]
        [InlineData(10000)]
        [InlineData(10034)]
        [InlineData(100099)]
        public async Task RemoveAsync_FailedDeletedProduct(int id)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Producto producto;

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    producto = await repository.GetAsync(u => u.Id == id);
                    var action = async() => await repository.RemoveAsync(producto);

                    //Assert
                    await Assert.ThrowsAsync<ArgumentNullException>(action);
                }

            }
        }


        [Theory]
        [InlineData(1,1,"Producto 1.0","Descripction 1.0",99.99,1)]
        [InlineData(10, 10, "Producto 10.0", "Descripction 10.0", 99.99, 1)]
        [InlineData(100, 100, "Producto 100.0", "Descripction 100.0", 99.99, 1)]
        [InlineData(1000, 1000, "Producto 1000.0", "Descripction 1000.0", 99.99, 1)]
        public async Task UpdateAsync_UpdatedCorrectProductData(int id, int categoryId,string productName, string description, double price, int stock)
        {
            using(var transaction = Fixture.Connection.BeginTransaction())
            {
                Producto producto = new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    ProductName = productName,
                    Description = description,
                    Price = price,
                    Stock = stock,
                    DateCaducated = DateTime.Now
                };

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    await repository.UpdateAsync(producto);
                    
                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    var response = await repository.GetAsync(u => u.Id == id);

                    //Assert
                    Assert.Equal(id,response.Id);
                    Assert.Equal(productName, response.ProductName);
                    Assert.Equal(description, response.Description);
                    Assert.Equal(price, response.Price);
                }
            }
        }


        [Theory]
        [InlineData(1000, 10000, "Producto 1000.0", "Descripction 1000.0", 99.99, 1)]
        [InlineData(1000, 1000, null, "Descripction 1000.0", 99.99, 1)]
        [InlineData(1000, 1000, "Producto 1000.0", null, 99.99, 1)]
        public async Task UpdateAsync_FailedUpdateProductData_ThrowsDbUpdateException(int id, int categoryId, string productName, string description, double price, int stock)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Producto producto = new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    ProductName = productName,
                    Description = description,
                    Price = price,
                    Stock = stock,
                    DateCaducated = DateTime.Now
                };

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    var action = async() => await repository.UpdateAsync(producto);

                    //Assert
                    await Assert.ThrowsAsync<DbUpdateException>(action);
                }

            }
        }

        [Theory]
        [InlineData(10001, 1000, "Producto 1000.0", "Descripction 1000.0", 99.99, 1)]
        [InlineData(10002, 1000, "Producto 1000.0", "Descripction 1000.0", 99.99, 1)]
        [InlineData(10003, 1000, "Producto 1000.0", "Descripction 1000.0", 99.99, 1)]
        public async Task UpdateAsync_ProductDoesntExist_ThrowException(int id, int categoryId, string productName, string description, double price, int stock)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Producto producto = new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    ProductName = productName,
                    Description = description,
                    Price = price,
                    Stock = stock,
                    DateCaducated = DateTime.Now
                };

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new ProductRepository(context);

                    //Act
                    var action = async () => await repository.UpdateAsync(producto);

                    //Assert
                    await Assert.ThrowsAsync<DbUpdateConcurrencyException>(action);
                }

            }
        }
    }

}
