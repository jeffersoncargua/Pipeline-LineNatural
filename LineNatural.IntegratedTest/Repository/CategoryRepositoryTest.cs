using LineNatural.Entities;
using LineNatural.Repository;
using Microsoft.EntityFrameworkCore;

namespace LineNatural.IntegratedTest.Repository
{
    [Collection("My Collection")]
    //public class CategoryRepositoryTest : IClassFixture<SharedDatabaseFixture> // IClassFixtute -> permite compartir la base de datos configurada
                                                                                 // en SharedDatabase entre las pruebas de esta clase
    public class CategoryRepositoryTest  
    {
        private SharedDatabaseFixture Fixture { get; } // Permite instanciar la clase SharedDatabaseFixture para utilizar el metodo CreateContext
                                                       // ademas en caso de realizar transacciones permite utilizar BeginTransaction() para
                                                       // emplear la misma base de datos con los cambios que se realicen con los metodos Get, Create,
                                                       // Update, Delete de los repositorios que deseamos probar; es decir; permite mutar la base de datos
        public CategoryRepositoryTest(SharedDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new CategoryRepository(context);

                //Act
                var response = await repository.GetAllAsync();

                //Assert
                Assert.IsType<List<Category>>(response);
                Assert.Equal(1000, response.Count);
            }
        }

        [Theory]
        [InlineData("Category 1")]
        [InlineData("Category 10")]
        [InlineData("Category 100")]
        [InlineData("Category 1000")]
        public async Task GetAllAsync_ReturnsCategoriesWithFilters(string search)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new CategoryRepository(context);

                //Act
                var response = await repository.GetAllAsync(u => u.CategoryName.Contains(search));

                //Assert
                Assert.Contains(search, response.First().CategoryName);
            }
        }

        [Theory]
        [InlineData("Category 1000000")]
        public async Task GetAllAsync_ShouldFailReturnItems(string search)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new CategoryRepository(context);

                //Act
                var response = await repository.GetAllAsync(u => u.CategoryName.Contains(search));

                //Assert
                Assert.Equal(0, response.Count);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task GetAsync_ReturnCategoryById(int id)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new CategoryRepository(context);

                //Act
                var response = await repository.GetAsync(u => u.Id == id);

                //Assert
                Assert.Equal(id, response.Id);
                Assert.IsType<Category>(response);
            }
        }

        [Theory]
        [InlineData("Category 1")]
        [InlineData("Category 10")]
        [InlineData("Category 100")]
        [InlineData("Category 1000")]
        public async Task GetAsync_ReturnCategoryWithFilter(string search)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new CategoryRepository(context);

                //Act
                var response = await repository.GetAsync(u => u.CategoryName.Contains(search));

                //Assert
                Assert.Contains(search, response.CategoryName);
                Assert.IsType<Category>(response);
                Assert.NotNull(response);
            }
        }

        [Theory]
        [InlineData(1001)]
        [InlineData(10021)]
        [InlineData(1003)]
        public async Task GetAsync_ReturnEmptyById(int id)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new CategoryRepository(context);

                //Act
                var response = await repository.GetAsync(u => u.Id == id);

                //Assert
                Assert.Null(response);
            }
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(1, true)]
        [InlineData(10, false)]
        [InlineData(10, true)]
        [InlineData(100, false)]
        [InlineData(100, true)]
        [InlineData(1000, false)]
        [InlineData(1000, true)]
        public async Task GetAsync_ReturnCategoryByIdWithTracked(int id, bool tracked)
        {
            using (var context = Fixture.CreateContext())
            {
                //Arrange
                var repository = new CategoryRepository(context);

                //Act
                var response = await repository.GetAsync(u => u.Id == id, tracked);

                //Assert
                Assert.Equal(id, response.Id);
            }

        }

        [Theory]
        [InlineData("Category 1001")]
        [InlineData("Category 1002")]
        [InlineData("Category 1000")]
        public async Task CreateAsync_SavedCorrectCategoryData(string categoryName)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Category category = new()
                {
                    CategoryName = categoryName
                };
                
                using (var context = Fixture.CreateContext(transaction)) 
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    await repository.CreateAsync(category);

                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    var response = await repository.GetAsync(u => u.CategoryName == categoryName);

                    //Assert
                    Assert.Equal(categoryName, response.CategoryName);
                    Assert.NotNull(response);
                }
            }
        }


        [Theory]
        [InlineData(null)]
        public async Task CreateAsync_FailedSaveCategoryData(string categoryName)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Category category = new()
                {
                    CategoryName = categoryName
                };

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    var action = async() => await repository.CreateAsync(category);

                    //Assert

                    await Assert.ThrowsAnyAsync<Exception>(action);

                }  
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task RemoveAsync_DeletedCorrectCategory(int id)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Category category;

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    category = await repository.GetAsync(u => u.Id == id);
                    await repository.RemoveAsync(category);

                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    var response = await repository.GetAsync(u => u.Id == id);

                    //Assert
                    Assert.Null(response);

                }
            }
        }

        [Theory]
        [InlineData(10005)]
        [InlineData(10045)]
        [InlineData(1003)]
        [InlineData(1002)]
        public async Task RemoveAsync_FailDeleteCorrectCategory(int id)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Category category;

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    category = await repository.GetAsync(u => u.Id == id);
                    var action = async() => await repository.RemoveAsync(category);

                    //Assert
                    await Assert.ThrowsAnyAsync<ArgumentNullException>(action);

                }

            }
        }

        [Theory]
        [InlineData(1, "Category Name 1.0")]
        [InlineData(10, "Category Name 10.0")]
        [InlineData(100, "Category Name 100.0")]
        [InlineData(1000, "Category Name 1000.0")]
        public async Task UpdateAsync_UpdatedCorrectCategoryData(int id, string categoryName)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Category category = new()
                {
                    Id = id,
                    CategoryName = categoryName
                };

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    await repository.UpdateAsync(category);
                }

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    var response = await repository.GetAsync(u =>u.Id == id);

                    //Assert
                    Assert.Equal(id, response.Id);
                    Assert.Equal(categoryName, response.CategoryName);
                }
            }
        }

        [Theory]
        [InlineData(1005, "Category Name 1000.0")]
        [InlineData(1001, "Category Name 1000.0")]
        [InlineData(10023, "Category Name 1000.0")]
        [InlineData(1003, "Category Name 1000.0")]
        [InlineData(1000, null)]
        public async Task UpdateAsync_FailedUpdateCategoryData(int id, string categoryName)
        {
            using (var transaction = Fixture.Connection.BeginTransaction())
            {
                Category category = new()
                {
                    Id = id,
                    CategoryName = categoryName
                };

                using (var context = Fixture.CreateContext(transaction))
                {
                    //Arrange
                    var repository = new CategoryRepository(context);

                    //Act
                    var  action = async() => await repository.UpdateAsync(category);

                    //Assert
                    await Assert.ThrowsAnyAsync<DbUpdateException>(action);
                }   
            }
        }
    }
}
