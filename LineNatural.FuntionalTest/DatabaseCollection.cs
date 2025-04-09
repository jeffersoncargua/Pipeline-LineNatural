using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace LineNatural.FuntionalTest
{
    [CollectionDefinition("My Collection 2")]
    public class DatabaseCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
    {

    }
}
