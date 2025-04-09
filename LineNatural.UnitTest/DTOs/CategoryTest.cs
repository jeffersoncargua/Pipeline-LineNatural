using LineNatural.DTOs.Category;

namespace LineNatural.UnitTest.DTOs
{
    public class CategoryTest : BaseTest
    {
        [Theory]
        [InlineData(null,1)]
        [InlineData("iewruweinfeiufnecifucfixuylwiufjyckliueyjieuyeiuyjlciryjiucieyifhmnmhfml", 1)]
        [InlineData("Sales Minerales", 0)]
        public void ValidModel_CategoryCreateDto_ReturnsCorrectNumberOfErrors(string categoryName, int errorExpected)
        {
            //Arrange
            CategoryCreateDto categoryCreateDto = new()
            {
                CategoryName = categoryName
            };

            //Act
            var errorsResult = ValidateModel(categoryCreateDto);

            //Assert
            Assert.Equal(errorExpected, errorsResult.Count);
        }


        [Theory]
        [InlineData(null,null,2)]
        [InlineData(1, null, 1)]
        [InlineData(2, "oiwejfiwecrkefne,ijcyecliueycfieujyfceiufylixfueycuekc", 1)]
        [InlineData(null, "oiwejfiwecrkefne,ijcyecliueycfieujyfceiufylixfueycuekc", 2)]
        [InlineData(3, "Sales Minerales", 0)]
        public void ValidModel_CategoryUpdateDto_ReturnsCorrectNumberOfErrors(int id, string? categoryName, int errorExpected)
        {
            //Arrange
            CategoryUpdateDto categoryUpdateDto = new()
            {
                Id = id,
                CategoryName = categoryName
            };

            //Act
            var errorsResult = ValidateModel(categoryUpdateDto);

            //Assert
            Assert.Equal(errorExpected, errorsResult.Count);
        }

        [Theory]
        [InlineData(null, null, 2)]
        [InlineData(1, null, 1)]
        [InlineData(2, "oiwejfiwecrkefne,ijcyecliueycfieujyfceiufylixfueycuekc", 1)]
        [InlineData(null, "oiwejfiwecrkefne,ijcyecliueycfieujyfceiufylixfueycuekc", 2)]
        [InlineData(3, "Sales Minerales", 0)]
        public void ValidModel_CategoryDto_ReturnsCorrectNumberOfErrors(int id, string? categoryName, int errorExpected)
        {
            //Arrange
            CategoryDto categoryDto = new()
            {
                Id = id,
                CategoryName = categoryName
            };

            //Act
            var errorsResult = ValidateModel(categoryDto);

            //Assert
            Assert.Equal(errorExpected, errorsResult.Count);
        }
    }
}
