
namespace LineNatural.FuntionalTest.Controllers
{
    [Collection("My Collection 2")]
    //public class BaseControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    public class BaseControllerTests 
    {
        //CustomWebApplicationFactory es una clase en C# que permite crear una fábrica para
        //iniciar una aplicación en memoria para realizar pruebas de integración y/o funcional. 
        private readonly CustomWebApplicationFactory<Program> _factory; //Se instancia la fabrica que permitira
                                                                        //utilizar los datos almacenados en memoria

        public BaseControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory; //Permite utilizar los datos almacenados en memoria
        }

        /// <summary>
        /// Permite instanciar un cliente para que pueda efectar acciones HTTP, ademas tiene acceso a la informacion
        /// de la base de datos
        /// </summary>
        /// <returns>Retorna un cliente HTTP</returns>
        public HttpClient GetNewClient()
        {
            var newClient = _factory.WithWebHostBuilder(builder =>
            {
                _factory.CustomConfigureServices(builder);
            }).CreateClient();

            return newClient;
        }
    }
}
