
namespace LineNatural.IntegratedTest
{
    [CollectionDefinition("My Collection")] //Permite definir la coleccion de datos que se va a compartir entre clases
                                            //de prueba, para nuestro caso CategoryRepositoryTest y ProductRepositoryTest
    public class DatabaseCollection : ICollectionFixture<SharedDatabaseFixture>
    {

    }
}
