using LineNatural.DTOs.Category;
using LineNatural.Entities;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LineNatural.FuntionalTest.Controllers
{
    public class CategoryControllerTest : BaseControllerTests 
    {
        public CategoryControllerTest(CustomWebApplicationFactory<Program> factory) : base(factory)
        {

        }

        [Fact]
        public async Task GetCategories_ReturnStatusOk()
        {
            //Arrange
            var client = this.GetNewClient(); //Permite instanciar el cliente para utilizar el metodo GetAsync

            //Act
            var response = await client.GetAsync("/api/Category/"); //pemite realizar la peticion al nuestro api del controlador Category
            response.EnsureSuccessStatusCode(); //Permite asegurar que el statusCode sera OK o Success

            var stringResponse = await response.Content.ReadAsStringAsync(); //Permite leer el content de la respuesta que se ha enviado desde la API
            var result = JsonConvert.DeserializeObject<IEnumerable<Category>>(stringResponse).ToList();//Permite deserializar el contenido para obtener
                                                                                                       //la lista con las categorias
            var statusCode = response.StatusCode.ToString(); // permite obtener el statusCOde de la respuesta y transformarlo a string para su lectura

            //Assert
            Assert.Equal("OK", statusCode);
            Assert.True(result.Count == 1000);
        }

        [Theory]
        [InlineData("Category 1")]
        [InlineData("Category 10")]
        [InlineData("Category 100")]
        [InlineData("Category 1000")]
        public async Task GetCategoriesWithFilter_ReturnStatusOk(string search)
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync($"/api/Category?search={search}");
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<Category>>(stringResponse).ToList();
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("OK", statusCode);
            Assert.Contains(search, result.First().CategoryName);
            Assert.IsType<List<Category>>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task GetCategoryById_ReturnCategory_StatusOk(int id)
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync($"api/Category/{id}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result =JsonConvert.DeserializeObject<Category>(stringResponse);
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("OK", statusCode);
            Assert.IsType<Category>(result);
        }


        [Theory]
        [InlineData(1, "Category 1")]
        [InlineData(10, "Category 10")]
        [InlineData(100, "Category 100")]
        [InlineData(1000, "Category 1000")]
        public async Task GetCategoryByIdAndFilter_ReturnCategory_StatusOk(int id, string search)
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync($"api/Category/{id}/?search={search}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Category>(stringResponse);
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("OK", statusCode);
            Assert.IsType<Category>(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-100)]
        [InlineData(-1000)]
        public async Task GetCategoryBy_ShouldReturnInteralServerError(int id) 
        { 
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync($"api/Category/{id}");
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("InternalServerError", statusCode);
        }


        [Theory]
        [InlineData(10000, null)]
        [InlineData(10000000,"Category 1002")]
        [InlineData(100000000,null)]
        [InlineData(100000000,"Category 1001")]
        public async Task GetCategory_WhenCategoryDoesntExist_ReturnStatusNotFound(int id, string search)
        {
            //Arrange
            var client = this.GetNewClient();

            //Act
            var response = await client.GetAsync($"api/Category/{id}/?search={search}");
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("NotFound", statusCode);
        }

        [Theory]
        [InlineData("Category 1001")]
        [InlineData("Category 1002")]
        [InlineData("Category 1003")]
        public async Task PostCategory_SavedCorrect_ReturnRouteWithNewCategory(string categoryName)
        {
            var client = this.GetNewClient();

            //Configuracion para crear un nuevo item
            
            //Arrange
            CategoryCreateDto categoryCreateDto = new() //se instancia el CategoryCreateDto para enviarlo a traves del client HTTP Post
            {
                CategoryName = categoryName,
            };

            //Se debe serializar el objeto ya que se debe enviar con un formato JSON por lo que se emplea la clase StringContent con los 
            //parametros que se indican en la siguiente linea
            var stringContent = new StringContent( JsonConvert.SerializeObject(categoryCreateDto),Encoding.UTF8,"application/json");

            //Act
            var response1 = await client.PostAsync("api/Category/PostCategory", stringContent);

            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CategoryDto>(stringResponse1);
            var statusCode = response1.StatusCode.ToString();

            //Assert
            Assert.Equal("Created", statusCode);

            //Configuracion para verificar que se haya generado el item en la base de datos de prueba

            //Act
            var response2 = await client.GetAsync($"api/Category/{result.Id}");
            response2.EnsureSuccessStatusCode();
            var stringResponse2 = await response2.Content.ReadAsStringAsync();
            var result2 = JsonConvert.DeserializeObject<Category>(stringResponse2);
            var statusCode2 = response2.StatusCode.ToString();

            //Assert
            Assert.Equal("OK",statusCode2);
            Assert.Equal(result.Id, result2.Id);
            Assert.Equal(result.CategoryName, result2.CategoryName);
        }

        [Theory]
        [InlineData("Category 1")]
        [InlineData("Category 10")]
        [InlineData("Category 100")]
        [InlineData("Category 1000")]
        public async Task PostCategory_CategoryExist_ReturnStatusInternalServerError(string categoryName)
        {
            //Arrange
            var client = this.GetNewClient();
            CategoryCreateDto categoryCreateDto = new()
            {
                CategoryName = categoryName
            };

            var stringContent = new StringContent( JsonConvert.SerializeObject(categoryCreateDto),Encoding.UTF8,"application/json");

            //Act
            var response1 = await client.PostAsync("/api/Category/PostCategory", stringContent);

            var statusCode = response1.StatusCode.ToString();

            //Assert
            Assert.Equal("InternalServerError", statusCode);
        }


        /// <summary>
        /// Permite probar si se envia un CategoryCreateDto null al api/Category/PostCategory
        /// Importante: Si se envian parametros que no sean validos por DataAnnotation se enviara
        /// un BadrRequest sin entrar a las funcionalidades del contralador
        /// </summary>
        /// <returns>Retorna un BadRequest en caso de CategoryCreateDto=null</returns>
        [Fact]
        public async Task PostCategory_CategoryNull_ReturnStatusBadRequest()
        {
            //Arrange
            var client = this.GetNewClient();

            CategoryCreateDto categoryCreateDto = null;

            var stringContent = new StringContent(JsonConvert.SerializeObject(categoryCreateDto), Encoding.UTF8,"application/json");

            //Act
            var response = await client.PostAsync("/api/Category/PostCategory", stringContent);
            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("BadRequest", statusCode);
        }

        [Theory]
        [InlineData(1,"Category 1.0")]
        [InlineData(10, "Category 10.0")]
        [InlineData(100, "Category 100.0")]
        [InlineData(1000, "Category 1000.0")]
        public async Task PutCategory_UpdatedCorrectCategoryData_ReturnNoContent(int id, string categoryName)
        {
            var client = this.GetNewClient();
            CategoryUpdateDto categoryUpdateDto = new()
            {
                Id = id,
                CategoryName = categoryName
            };
            //Configuracion para realizar el proceso de actualizacion de Category
            //Arrange


            var stringContent = new StringContent(JsonConvert.SerializeObject(categoryUpdateDto),Encoding.UTF8,"application/json");

            //Act
            var response1 = await client.PutAsync($"/api/Category/{id}", stringContent);

            var statusCode = response1.StatusCode.ToString();

            //Assert
            Assert.Equal("NoContent", statusCode);

            
            //Configuracion para verificar que se haya modificado la category

            //Act
            var response2 = await client.GetAsync($"/api/Category/{id}");
            response2.EnsureSuccessStatusCode();

            var stringResponse2 = await response2.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CategoryDto>(stringResponse2);
            var statusCode2 = response2.StatusCode.ToString();


            //Assert
            Assert.Equal(categoryUpdateDto.Id, result.Id);
            Assert.Equal(categoryUpdateDto.CategoryName, result.CategoryName);
            Assert.Equal("OK", statusCode2);
        }

        /// <summary>
        /// Permite evaluar que los parametros ingresados se validen con los DataAnnotation de CategoryUpdateDto
        /// Importante: Cuando se quiera acceder al api/Category/PutCategory este enviara directamente un BadRequest
        /// en el caso de que se envien mal los parametros de CategoryUpdateDto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryName"></param>
        /// <returns>Retorna un BadRequest en caso de no cunplir con las validaciones de DataAnnotation</returns>
        [Theory]
        [InlineData(-1, "Category 1.0")]
        [InlineData(10, null)]
        [InlineData(-100, "Category 100.0")]
        [InlineData(null, null)]
        public async Task PutCategory_NullOrInvalidParameter_ReturnStatusBadRequest(int id, string categoryName)
        {
            //Arrange
            var client = this.GetNewClient();
            CategoryUpdateDto categoryUpdateDto = new()
            {
                Id = id,
                CategoryName = categoryName
            };
   
            var stringContent = new StringContent(JsonConvert.SerializeObject(categoryUpdateDto),Encoding.UTF8,"application/json");

            //Act
            var response = await client.PutAsync($"/api/Category/{id}", stringContent);

            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal("BadRequest", statusCode);
        }


        [Theory]
        [InlineData(1000, 1, "Category 1.0",HttpStatusCode.BadRequest)]
        [InlineData(100, 10, "Category 1000.0",HttpStatusCode.BadRequest)]
        [InlineData(-10, 100, "Category 1000.0",HttpStatusCode.InternalServerError)]
        [InlineData(-1, 1000, "Category 1000.0",HttpStatusCode.InternalServerError)]
        [InlineData(1002, 1002, "Category 1000.0", HttpStatusCode.NotFound)]
        [InlineData(1001, 1001, "Category 1000.0", HttpStatusCode.NotFound)]
        public async Task PutCategory_IncorrectIds_ShouldReturnStatusBadRequestNotFoundAndInternalServerError(int id, int categoryId, string categoryName, HttpStatusCode code)
        {
            //Arrange
            var client = this.GetNewClient();
            CategoryUpdateDto categoryUpdateDto = new()
            {
                Id = categoryId,
                CategoryName = categoryName
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(categoryUpdateDto), Encoding.UTF8, "application/json");

            //Act
            var response = await client.PutAsync($"/api/Category/{id}", stringContent);

            var statusCode = response.StatusCode.ToString();

            //Assert
            Assert.Equal(code.ToString(), statusCode);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task DeleteCategory_DeletedCorrectCategory_ReturnStatusNotFound(int id)
        {
            var client = this.GetNewClient();

            //Arrange
            var response1 = await client.DeleteAsync($"api/Category/{id}");
            var statusCode = response1.StatusCode.ToString();

            //Assert
            Assert.Equal("NoContent", statusCode);


            //Act
            var response2 = await client.GetAsync($"api/Category/{id}");
            var statusCode2 = response2.StatusCode.ToString();

            //Assert
            Assert.Equal("NotFound", statusCode2);
        }


        [Theory]
        [InlineData(1001,HttpStatusCode.NotFound)]
        [InlineData(-10, HttpStatusCode.InternalServerError)]
        [InlineData(-1001, HttpStatusCode.InternalServerError)]
        [InlineData(1004, HttpStatusCode.NotFound)]
        public async Task DeleteCategory_CategoryDoesntExistAndInvalidId_ShouldReturnNotFoundOrInternalServerError(int id, HttpStatusCode code)
        {
            var client = this.GetNewClient();

            //Arrange
            var response1 = await client.DeleteAsync($"api/Category/{id}");
            var statusCode = response1.StatusCode.ToString();

            //Assert
            Assert.Equal(code.ToString(), statusCode);

        }
    }
}
