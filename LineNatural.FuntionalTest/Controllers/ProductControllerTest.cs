using LineNatural.DTOs.Product;
using LineNatural.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace LineNatural.FuntionalTest.Controllers
{
    public class ProductControllerTest : BaseControllerTests 
    {
        public ProductControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {

        }

        [Fact]
        public async Task GetAllAsync_ReturnAllProductosAndStatusOk()
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync("api/Product");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<Producto>>(stringResponse).ToList();
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("OK", statusCode);
            Assert.True(result.Count == 1000);
        }

        [Theory]
        [InlineData("Producto 1")]
        [InlineData("Producto 10")]
        [InlineData("Producto 1000")]
        [InlineData("Producto 100")]
        public async Task GetProducts_ProductsWithFilter_ReturnStatusOk(string search)
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync($"/api/Product?search={search}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<ProductoDto>>(stringResponse).ToList();
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("OK", statusCode);
            Assert.Contains(search, result.Last().ProductName);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task GetProduct_ReturnProductoByIdAndStatusOK(int id)
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync($"/api/Product/{id}");
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ProductoDto>(stringResponse);
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("OK", statusCode);
            Assert.True(result.Id == id);
        }


        [Theory]
        [InlineData(1, "Producto 1", HttpStatusCode.OK)]
        [InlineData(10, "Producto 10", HttpStatusCode.OK)]
        [InlineData(100, "Producto 10000", HttpStatusCode.NotFound)]
        [InlineData(10000, "Producto 100", HttpStatusCode.NotFound)]
        [InlineData(-1000, "Producto 1000", HttpStatusCode.InternalServerError)]
        [InlineData(-100, "Producto 1000", HttpStatusCode.InternalServerError)]
        public async Task GetProduct_ReturnProductByFilter_ShouldReturnOKNotFoundOrInternalServerError(int id, string search, HttpStatusCode code)
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync($"/api/Product/{id}/?search={search}");
            var strigReponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ProductoDto>(strigReponse);
            var statusCode = response.StatusCode;

            //Assert
            Assert.Equal(code, statusCode);
        }


        [Theory]
        [InlineData(1,"Producto 1001","Description 1001",1.98,3)]
        public async Task PostProduct_WhenSavedCorrectProductData_ReturnStatusCreated(int categoryId, string productName, string description,double price, int stock)
        {

            var client = this.GetNewClient();
            ProductoCreateDto productoCreateDto = new()
            {
                CategoryId = categoryId,
                ProductName = productName,
                Description = description,
                Price = price,
                Stock = stock
            };
            
            //Configuracion para agregar un nuevo producto en la base de datos
            //Arrange
            var stringContent = new StringContent(JsonConvert.SerializeObject(productoCreateDto), Encoding.UTF8, "application/json");

            //Act
            var response1 = await client.PostAsync("api/Product/PostProduct", stringContent);

            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ProductoDto>(stringResponse1);
            var statusCode = response1.StatusCode.ToString();

            //Assert
            Assert.Equal("Created",statusCode);

            //Configuracion para buscar el nuevo producto en la base de datos

            //Act
            var response2 = await client.GetAsync($"/api/Product/{result.Id}");
            var stringResponse2 = await response2.Content.ReadAsStringAsync();
            var resultExpected = JsonConvert.DeserializeObject<ProductoDto>(stringResponse2);
            var statusCode2 = response2.StatusCode.ToString();

            //Assert
            Assert.Equal("OK", statusCode2);
            Assert.True(result.Id == resultExpected.Id);
            Assert.True(result.ProductName == resultExpected.ProductName);
            Assert.True(result.Stock == resultExpected.Stock);
            Assert.IsType<ProductoDto>(resultExpected);
            Assert.IsType<ProductoDto>(result);
        }


        [Theory]
        [InlineData(1001, "Producto 1001", "Description 1001", 1.98, 3)]
        [InlineData(1, null, "Description 1001", 1.98, 3)]
        [InlineData(1, "Producto 1001", null, 1.98, 3)]
        [InlineData(1, "Producto 1001", "Description 1001", -1.98, 3)]
        [InlineData(-1, "Producto 1001", "Description 1001", 1.98, -3)]
        public async Task PostProduct_WhenSendInvalidParameters_ReturnStatusBadReques(int categoryId, string productName, string description, double price, int stock)
        {
            var client = this.GetNewClient();

            //Arrange
            ProductoCreateDto productoCreateDto = new()
            {
                CategoryId = categoryId,
                ProductName = productName,
                Description = description,
                Price = price,
                Stock = stock
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(productoCreateDto), Encoding.UTF8, "application/json");


            //Act
            var response = await client.PostAsync("/api/Product/PostProduct",stringContent);
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("BadRequest", statusCode);
        }


        [Theory]
        [InlineData(1, 1, 1, "Producto 1.0", "Description 1.0", 1.98, 3)]
        [InlineData(10, 10, 10, "Producto 10.0", "Description 10.0", 14.98, 8)]
        [InlineData(100, 100, 100, "Producto 100.0", "Description 100.0", 23.98, 1)]
        [InlineData(1000, 1000, 1000, "Producto 1000.0", "Description 1000.0", 99.9, 23)]
        public async Task PutProduct_WhenUpdatedCorrectProductData_ReturnNoContent(int id,int productId,int categoryId,string productName, string description, double price,int stock)
        {
            var client = this.GetNewClient();

            //Configuracion para actualizar un producto
            //Arrange
            ProductoUpdateDto productoUpdateDto = new()
            {
                Id = productId,
                CategoryId = categoryId,
                ProductName = productName,
                Description = description,
                Price = price,
                Stock = stock
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(productoUpdateDto),Encoding.UTF8, "application/json");

            //Act
            var response1 = await client.PutAsync($"/api/Product/{id}",stringContent);
            var statusCode = response1.StatusCode.ToString();

            //Assert
            Assert.Equal("NoContent", statusCode);

            //Configuracion para verificar que se haya actualizado el producto
            //Act
            var response2 = await client.GetAsync($"/api/Product/{id}");
            var stringResponse = await response2.Content.ReadAsStringAsync();
            var resultExpected = JsonConvert.DeserializeObject<ProductoDto>(stringResponse);
            var statusCode2 = response2.StatusCode.ToString();

            //Assert
            Assert.Equal("OK", statusCode2);
            Assert.True(productId == resultExpected.Id);
            Assert.True(categoryId == resultExpected.CategoryId);
            Assert.True(productName == resultExpected.ProductName);
            Assert.True(description == resultExpected.Description);

        }

        [Theory]
        [InlineData(-1, 1, 1, "Producto 1.0", "Description 1.0", 1.98, 3,HttpStatusCode.InternalServerError)]
        [InlineData(1, 1, 1001, "Producto 1.0", "Description 1.0", 1.98, 3,HttpStatusCode.BadRequest)]
        [InlineData(1, 10, 100, "Producto 1.0", "Description 1.0", 1.98, 3,HttpStatusCode.BadRequest)]
        [InlineData(1001, 1001, 1, "Producto 1.0", "Description 1.0", 1.98, 3,HttpStatusCode.NotFound)]
        public async Task PutProduct_WhenSendInlavidIds_ShouldReturnNotFoundBadRequestOrInternalServerError(int id, int productId, int categoryId, string productName, 
            string description, double price, int stock,HttpStatusCode code)
        {
            var client = this.GetNewClient();

            //Arrange
            ProductoUpdateDto productoUpdateDto = new()
            {
                Id = productId,
                CategoryId = categoryId,
                ProductName = productName,
                Description = description,
                Price = price,
                Stock = stock
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(productoUpdateDto),Encoding.UTF8,"application/json");

            //Act
            var response = await client.PutAsync($"/api/Product/{id}", stringContent);
            var statusCode = response.StatusCode;

            //Assert
            Assert.Equal(code, statusCode);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task DeleteProduct_WhenDeletedCorrectProduct_ReturnNoContent(int id)
        {
            //Configuracion para eliminar un producto

            //Arrange
            var client = this.GetNewClient();

            //Act
            var response1 = await client.DeleteAsync($"/api/Product/{id}");
            var statusCode = response1.StatusCode.ToString();

            //Assert
            Assert.Equal("NoContent",statusCode);

            //Configuracion para confirmar que se elimino el producto

            //Act
            var response2 = await client.GetAsync($"/api/Product/{id}");
            var statusCode2 = response2.StatusCode.ToString();

            //Assert
            Assert.Equal("NotFound", statusCode2);
        }


        [Theory]
        [InlineData(-1,HttpStatusCode.InternalServerError)]
        [InlineData(10001,HttpStatusCode.NotFound)]
        public async Task DeleteProduct_WhenSendInvalidIds_ShouldReturnNotFoundOrInternalServerError(int id,HttpStatusCode code)
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response1 = await client.DeleteAsync($"/api/Product/{id}");
            var statusCode = response1.StatusCode;

            //Assert
            Assert.Equal(code, statusCode);
        }

    }
}
