using LineNatural.DTOs.Product;
using LineNatural.Entities;

namespace LineNatural.UnitTest.DTOs
{
    public class ProductTest : BaseTest
    {
        [Theory]
        [InlineData(null, null, null, null, -1.0, -1 ,6)]
        [InlineData(-1, Int32.MaxValue, null, null, 10000.0, 101, 6)]
        [InlineData(0, 0, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -1.0, -1, 6)]
        [InlineData(Int32.MaxValue, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -1.0, 0, 5)]
        [InlineData(Int32.MaxValue, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, 1000.0, 0, 5)]
        [InlineData(Int32.MaxValue, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, 1000.0, -1, 6)]
        [InlineData(Int32.MaxValue, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -10.0, 1000, 6)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -10.0, 1000, 5)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -1.0, -1, 4)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", null, -10.0, -1, 3)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", -3, 102, 2)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", -1.0, 0, 1)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", 0.1, -1, 1)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", 0.1, 10000, 1)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", 0.1, 0, 0)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", 0.1, 100, 0)]
        public void ValidateModel_ProductoDto_ReturnsNumberOfErrors(int id, int categoryId, string? productName, string? description, double price, int stock ,int errorsExpected)
        {
            //Arrange
            ProductoDto productoDto = new()
            {
                Id = id,
                CategoryId = categoryId,
                ProductName = productName,
                Description = description,
                Price = price,
                Stock = stock,
                DateCaducated = DateTime.Now
            };


            //Act
            var errorsResult = ValidateModel(productoDto);

            //Assert
            Assert.Equal(errorsExpected, errorsResult.Count);

        }


        [Theory]
        [InlineData(null,null,null,-10.0,-1,5)]
        [InlineData(-1, "weuiyreiemicfunemfieufmhxiucfhiefmerifxifcmxieeceirasuhquihsxiunwsxiuewuwexn", null, 1000.0, 101, 5)]
        [InlineData(Int32.MaxValue, "weuiyreiemicfunemfieufmhxiucfhiefmerifxifcmxieeceirasuhquihsxiunwsxiuewuwexn", null, 10.0, -1, 4)]
        [InlineData(Int32.MaxValue - 1 , "weuiyreiemicfunemfieufmhxiucfhiefmerifxifcmxieeceirasuhquihsxiunwsxiuewuwexn", null, 10.0, -1, 3)]
        [InlineData(Int32.MaxValue - 1, "Sales Minerales", null, 10.0, -1, 2)]
        [InlineData(Int32.MaxValue - 1, "Sales Minerales", "Lo que sea que se vaya a colocar", 10.0, -1, 1)]
        [InlineData(Int32.MaxValue - 1, "Sales Minerales", "Lo que sea que se vaya a colocar", 10.0, 1, 0)]
        public void ValidateModel_ProductoCreateDto_ReturnsNumberOfError(int categoryId, string productName, string description, double price, int stock, int errorsExpected)
        {
            //Arrange
            ProductoCreateDto productoCreateDto = new()
            {
                CategoryId = categoryId,
                ProductName = productName,
                Description = description,
                Price = price,
                Stock = stock
            };

            //Act
            var errorsResult = ValidateModel(productoCreateDto);

            //Assert
            Assert.Equal(errorsExpected, errorsResult.Count);
        }


        [Theory]
        [InlineData(null, null, null, null, -1.0, -1, 6)]
        [InlineData(-1, Int32.MaxValue, null, null, 10000.0, 101, 6)]
        [InlineData(0, 0, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -1.0, -1, 6)]
        [InlineData(Int32.MaxValue, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -1.0, 0, 5)]
        [InlineData(Int32.MaxValue, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, 1000.0, 0, 5)]
        [InlineData(Int32.MaxValue, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, 1000.0, -1, 6)]
        [InlineData(Int32.MaxValue, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -10.0, 1000, 6)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -10.0, 1000, 5)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "isuhfeuimfeiwcemiceimfemficenirierukierjukeokeoixweuroiemceuc", null, -1.0, -1, 4)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", null, -10.0, -1, 3)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", -3, 102, 2)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", -1.0, 0, 1)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", 0.1, -1, 1)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", 0.1, 10000, 1)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", 0.1, 0, 0)]
        [InlineData(Int32.MaxValue - 1, Int32.MaxValue - 1, "Sales Minereles", "Es un producto con sales minerales para el cuerpo", 0.1, 100, 0)]
        public void ValidateModel_ProductoUpdateDto_ReturnsNumberOfErrors(int id, int categoryId, string? productName, string? description, double price, int stock, int errorsExpected)
        {
            //Arrange
            ProductoUpdateDto productoUpdateDto = new()
            {
                Id = id,
                CategoryId = categoryId,
                ProductName = productName,
                Description = description,
                Price = price,
                Stock = stock
            };


            //Act
            var errorsResult = ValidateModel(productoUpdateDto);

            //Assert
            Assert.Equal(errorsExpected, errorsResult.Count);

        }

    }

}
