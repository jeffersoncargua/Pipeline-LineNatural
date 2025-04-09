using AutoMapper;
using Castle.Core.Configuration;
using LineNatural.Controllers;
using LineNatural.DTOs.LocalUser;
using LineNatural.Entities;
using LineNatural.Mapping;
using LineNatural.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text;
using Xunit.Sdk;

namespace LineNatural.FuntionalTest.Controllers
{
   
    public class UserControllerTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;
        public UserControllerTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _configuration = new MapperConfiguration( cfg =>
            {
                cfg.AddProfile<MappingConfig>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Theory]
        [InlineData("juan","juan@gmail.com","Admin123!","admin")]
        public async Task Register_WhenSendCorrectRequest_ReturnStatusCodeOK(string name, string userName, string password, string role)
        {
            //Arrange
            ApplicationUser user = new()
            {
                Name = name,
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            RegistrationRequestDto registrationRequestDto = new()
            {
                Name = name,
                UserName = userName,
                Password = password,
                Role = role
            };

            RegistrationResponseDto registrationResponse = new()
            {
                User = _mapper.Map<UserDto>(user),
                Token = "Aqui va el token para confirmar la cuenta por email"
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(registrationRequestDto), Encoding.UTF8,"application/json");

            var _mockClient = new MockHttpMessageHandler();

            _mockClient.When("https://localhost:7180/api/User/Register")
                .WithContent(JsonConvert.SerializeObject(registrationRequestDto))
                .Respond("application/json", JsonConvert.SerializeObject(registrationResponse));


            var client = _mockClient.ToHttpClient();

            _mockUserRepository.Setup(u => u.IsUnique(It.IsAny<string>())).Returns(true);
            _mockUserRepository.Setup(u => u.Registration(It.IsAny<RegistrationRequestDto>())).ReturnsAsync(registrationResponse);

            var controller = new UserController(_mockUserRepository.Object);

            //Act

            var clientResponse = await client.PostAsync("https://localhost:7180/api/User/Register", stringContent);
            var stringResponse = await clientResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RegistrationResponseDto>(stringResponse);

            var apiResponse = await controller.Register(registrationRequestDto);
            var response = apiResponse as ObjectResult;
            var result2 = response.Value as RegistrationResponseDto;
            var statusCode = response.StatusCode;

            //Assert
            Assert.Equal(result.Token, result2.Token);
            Assert.Equal(result.User.Id, result2.User.Id);
            Assert.True((int)clientResponse.StatusCode == (int)response.StatusCode);

        }


        [Theory]
        [InlineData("juan","juan@gmail.com", "Admin123!", "admin",true)]
        [InlineData("juan", "juan@gmail.com", "Admin123!", "admin", false)]
        public async Task Register_WhenUserExistsOrRegisterFiled_ReturnStatusCodeBadRequest(string name, string userName, string password, string role, bool userExist)
        {
            //Arrange
            RegistrationRequestDto registrationRequest = new()
            {
                UserName = userName,
                Password = password,
                Name = name,
                Role = role
            };

            var stringContent = new StringContent((JsonConvert.SerializeObject(registrationRequest)), Encoding.UTF8, "application/json");

            var _mockHttp = new MockHttpMessageHandler();
            _mockHttp.When("https://localhost:7180/api/User/Register")
                .WithContent(JsonConvert.SerializeObject(registrationRequest))
                .Respond(HttpStatusCode.BadRequest);

            var client = _mockHttp.ToHttpClient();

            _mockUserRepository.Setup(u => u.IsUnique(It.IsAny<string>())).Returns(userExist ? true : false);
            _mockUserRepository.Setup(u => u.Registration(It.IsAny<RegistrationRequestDto>())).Throws<InvalidDataException>();

            var controller = new UserController(_mockUserRepository.Object);
;
            //Act
            var clientResponse = await client.PostAsync("https://localhost:7180/api/User/Register",stringContent);
            var statusCodeResponse = clientResponse.StatusCode;

            var apiResponse = await controller.Register(registrationRequest);
            var response = apiResponse as BadRequestResult;
            var statusCode = response.StatusCode;

            //Assert
            Assert.True((int)response.StatusCode == (int)statusCodeResponse);

        }


        [Theory]
        [InlineData("Aqui va el token para confirmar el email", "Juan@gmail.com", true)]
        [InlineData("Aqui va el token para confirmar el email", "Juan@gmail.com", false)]
        public async Task ConfirmEmail_IfUserConfirmEmail_ShouldReturnStatusCodeOKOrBadRequest(string token, string userName, bool emailConfirm)
        {
            //Arrange
            var _mockHttp = new MockHttpMessageHandler();

            _mockHttp.When("https://localhost:7180/api/User/ConfirmEmail")
                .WithQueryString(new Dictionary<string, string>
                {
                    {"token", token },
                    {"email",userName }
                })
                .Respond(emailConfirm ? HttpStatusCode.OK: HttpStatusCode.BadRequest);

            var client = _mockHttp.ToHttpClient();

            _mockUserRepository.Setup(u => u.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(emailConfirm);

            var controller = new UserController(_mockUserRepository.Object);

            //Act
            var clientResponse = await client.GetAsync($"https://localhost:7180/api/User/ConfirmEmail?token={token}&email={userName}");
            var statusCodeResponse = clientResponse.StatusCode;

            var apiResponse = await controller.ConfirmEmail(token,userName);
            var response = apiResponse as StatusCodeResult;
            var statusCode = response.StatusCode;

            //Assert
            Assert.True((int)statusCodeResponse == statusCode);

        }


        [Theory]
        [InlineData("juan@gmail.com")]
        public async Task ForgetPassword_WhenSendCorrectForgetRequest_ReturnStatusCodeOk(string userName)
        {
            //Arrange
            ApplicationUser user = new()
            {
                Name = "Lo que sea",
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            ForgetRequestDto forgetRequest = new()
            {
                UserName = userName
            };

            ForgetResponseDto forgetResponse = new()
            {
                User = _mapper.Map<UserDto>(user),
                Token = "aqui va el token para cambiar la contraseña"
            };

            var stringContent = new StringContent((JsonConvert.SerializeObject(forgetRequest)),Encoding.UTF8, "application/json");

            var _mockHttp = new MockHttpMessageHandler();

            _mockHttp.When("https://localhost:7180/api/User/ForgetPassword")
                .WithContent(JsonConvert.SerializeObject(forgetRequest))
                .Respond("application/json",JsonConvert.SerializeObject(forgetResponse));

            var client = _mockHttp.ToHttpClient();

            _mockUserRepository.Setup(u => u.ForgetPassword(It.IsAny<ForgetRequestDto>())).ReturnsAsync(forgetResponse);
            var controller = new UserController(_mockUserRepository.Object);

            //Act
            var clientResponse = await client.PostAsync("https://localhost:7180/api/User/ForgetPassword", stringContent);
            var content = await clientResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ForgetResponseDto>(content);
            var statusCode = clientResponse.StatusCode;

            var apiResponse = await controller.ForgetPassword(forgetRequest);
            var response = apiResponse as ObjectResult;
            var result2 = response.Value as ForgetResponseDto;
            var statusCode2 = response.StatusCode;

            //Assert
            Assert.Equal((int)statusCode, statusCode2);
            Assert.Equal(result.Token, result2.Token);
        }

        [Fact]
        public async Task ForgetPassword_WhenSendUserNameDoesntExistsToRequest_ShouldReturnStatusCodeBadRequest()
        {
            //Arrange
            ForgetRequestDto forgetRequest = new()
            {
                UserName = "Loquese@gmail.com"
            };

            var stringContent = new StringContent((JsonConvert.SerializeObject(forgetRequest)),Encoding.UTF8, "application/json");
            
            var _mockClient = new MockHttpMessageHandler();

            _mockClient.When("https://localhost:7180/api/User/ForgetPassword")
                .Respond(HttpStatusCode.BadRequest);

            var client = _mockClient.ToHttpClient();

            var controller = new UserController(_mockUserRepository.Object);

            //Act
            var clientResponse = await client.PostAsync("https://localhost:7180/api/User/ForgetPassword", stringContent);
            var statusCode = clientResponse.StatusCode;

            var apiResponse = await controller.ForgetPassword(forgetRequest);
            var response = apiResponse as BadRequestResult;
            var statusCode2 = (HttpStatusCode)response.StatusCode;


            //Assert
            Assert.Equal(statusCode, statusCode2);
        }


        [Theory]
        [InlineData("juan","juan@gmail.com", "Admin123!")]
        public async Task Login_WhenRegistrationIsSuccess_ReturnLoginResponse(string name,string userName, string password)
        {
            //Arrange
            ApplicationUser user = new()
            {
                Name = name,
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            LoginRequestDto loginRequest = new()
            {
                UserName = userName,
                Password = password
            };

            LoginResponseDto loginResponse = new()
            {
                User = _mapper.Map<UserDto>(user),
                Token = "Aqui va el token para el login satisfactorio",
                Role = "admin"
            };

            var stringContent = new StringContent((JsonConvert.SerializeObject(loginRequest)),Encoding.UTF8,"application/json");

            var _mockHttp = new MockHttpMessageHandler();
            _mockHttp.When("https://localhost:7180/api/User/Login")
                .WithContent(JsonConvert.SerializeObject(loginRequest))
                .Respond("application/json", JsonConvert.SerializeObject(loginResponse));

            var client = _mockHttp.ToHttpClient();

            _mockUserRepository.Setup(u =>u.Login(It.IsAny<LoginRequestDto>())).ReturnsAsync(loginResponse);
            var controler = new UserController(_mockUserRepository.Object);

            //Act
            var clientResponse = await client.PostAsync("https://localhost:7180/api/User/Login", stringContent);
            clientResponse.EnsureSuccessStatusCode();
            var content = await clientResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponseDto>(content);
            var statusCode = clientResponse.StatusCode;

            var apiResponse = await controler.Login(loginRequest);
            var response = apiResponse as ObjectResult;
            var result2 = response.Value as LoginResponseDto;
            var statusCode2 = (HttpStatusCode)response.StatusCode;

            //Assert
            Assert.Equal(statusCode, statusCode2);
            Assert.NotNull(result.User);
            Assert.NotNull(result2.User);
        }


        [Theory]
        [InlineData("juan@gmail.com", "Admin123!")]
        public async Task Login_WhenRegistrationFailed_ReturnNotFound(string userName, string password)
        {
            //Arrange
            LoginRequestDto loginRequest = new()
            {
                UserName = userName,
                Password = password
            };

            var stringContent = new StringContent((JsonConvert.SerializeObject(loginRequest)),Encoding.UTF8, "application/json");

            var _mockHttp = new MockHttpMessageHandler();
            _mockHttp.When("https://localhost:7180/api/User/Login")
                .WithContent(JsonConvert.SerializeObject(loginRequest))
                .Respond(HttpStatusCode.NotFound);

            var client = _mockHttp.ToHttpClient();

            //_mockUserRepository.Setup(u => u.Login(It.IsAny<LoginRequestDto>()));
            var controller = new UserController(_mockUserRepository.Object);

            //Act
            var clientResponse = await client.PostAsync("https://localhost:7180/api/User/Login",stringContent);
            var statusCode = clientResponse.StatusCode;

            var apiResponse = await controller.Login(loginRequest);
            var response = apiResponse as NotFoundResult;
            var statusCode2 = (HttpStatusCode)response.StatusCode;

            //Assert
            Assert.True(statusCode == statusCode2);
        }
    }
}
