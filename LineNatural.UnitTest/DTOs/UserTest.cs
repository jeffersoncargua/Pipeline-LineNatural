using LineNatural.DTOs.LocalUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LineNatural.UnitTest.DTOs
{
    public class UserTest : BaseTest
    {
        [Theory]
        [InlineData("Juan", "juan@gmail.com","Admin123!", "admin",0)]
        [InlineData("Juan", "juan@gmail.com", "Admin123!", null, 0)]
        [InlineData(null, "juan", "Admin123!", "admin", 2)]
        [InlineData("Juaneircmueieocfienueicfemfcoieeuiyeidmiomwceufomeimueoiuicef", null, "Admin123!", "admin", 2)]
        [InlineData(null, null, null, "admin", 3)]
        public void ModelValid_RegistrationRequestDto_ReturnCorrectNumnerOfErrors(string name, string email, string password,string role, int errorsExpected)
        {
            //Arrange
            RegistrationRequestDto registrationRequestDto = new()
            {
                Name = name,
                UserName = email,
                Password = password,
                Role = role,
            };

            //Act
            var currentErrors = ValidateModel(registrationRequestDto);

            //Assert
            Assert.Equal(errorsExpected, currentErrors.Count);
        }


        [Theory]
        [InlineData("Juan@gmail.com", "Admin123!", 0)]
        [InlineData("Juan@gmail", "Admin123!", 1)]
        [InlineData("Juangmail.com", "Admin123!", 1)]
        [InlineData("Juansjdfheiujrdeiomfcueimfueoifeuoidfkfmiefoieu@gmail.com", null, 2)]
        [InlineData("Juansjdfheiujrdeiomfcueimfueoifeuoidfkfmiefoieu@gmail", null, 3)]
        [InlineData("Juan@gmail.com", null, 1)]
        public void ValidModel_LoginRequestDto_ReturnCorrectNumberOfErrors(string userName, string password, int errorsExpected)
        {
            //Arrange
            LoginRequestDto loginRequestDto = new()
            {
                UserName = userName,
                Password = password
            };

            //Act
            var currentErrors = ValidateModel(loginRequestDto);

            //Assert
            Assert.Equal(errorsExpected, currentErrors.Count);

        }


        [Theory]
        [InlineData("Juansjdfheiujrdeiomfcueimfueoifeuoidfkfmiefoieu@gmail", 2)]
        [InlineData("Juansjdfheiujrdeiomfcueimfueoifeuoidfkfmiefoieu@gmail.com", 1)]
        [InlineData(null, 1)]
        [InlineData("juan@gmail.com",0)]
        public void ValidModel_ForgetRequestDto_ReturnCorrectNumberOfErrors(string userName,int errorsExpected)
        {
            //Arrage
            ForgetRequestDto forgetRequestDto = new()
            {
                UserName = userName
            };

            //Act
            var currentErrors = ValidateModel(forgetRequestDto);

            //Assert
            Assert.Equal(errorsExpected, currentErrors.Count);
        }
    }
}

