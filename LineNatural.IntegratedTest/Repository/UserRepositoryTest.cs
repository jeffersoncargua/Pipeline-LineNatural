using AutoMapper;
using Bogus.DataSets;
using LineNatural.DTOs.LocalUser;
using LineNatural.Entities;
using LineNatural.Mapping;
using LineNatural.Repository;
using LineNatural.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Org.BouncyCastle.Asn1.Crmf;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace LineNatural.IntegratedTest.Repository
{
    public class UserRepositoryTest
    {
        //private SharedDatabaseFixture Fixture { get; }
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private IMapper _mapper;
        private readonly IConfigurationProvider _configuration;
        private readonly EmailConfiguration emailConfiguration;
        private readonly Mock<IConfigurationSection> _mockConfigurtionValue;
        private readonly Mock<IConfiguration> _mockConfiguration;
        public UserRepositoryTest()
        {
            //Fixture = fixture;

            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfig>();
            });

            _mapper = _configuration.CreateMapper();

            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object
                );

            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object
                );

            emailConfiguration = new EmailConfiguration()
            {
                From = "jeffersoncargua@gmail.com",
                SmtpServer = "smtp.gmail.com",
                Port = 465,
                UserName = "jeffersoncargua@gmail.com",
                Password = "ahvbblgyphdyrtce"
            };

            _mockConfigurtionValue = new Mock<IConfigurationSection>();
            _mockConfigurtionValue.Setup(x => x.Value).Returns("Aqui va la clave secreta");

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x.GetSection("APISettings:Secret")).Returns(_mockConfigurtionValue.Object);

        }

        [Theory]
        [InlineData("Juan","Juan@gmail.com","Admin1231!", "admin")]
        [InlineData("Cris", "Juan2@gmail.com", "Admin1232!", "admin")]
        [InlineData("Bryan", "Juan3@gmail.com", "Admin1233!", "costumer")]
        [InlineData("Yadira", "Juan4@gmail.com", "Admin1233!", "costumer")]
        public async Task Registration_WhenSendCorrectRegistrationRequest_ReturnRegistrationResponse(string name, string userName,string password, string role)
        {
            //Arrange
            RegistrationRequestDto registrationRequestDto = new()
            {
                Name = name,
                UserName = userName,
                Password = password,
                Role = role
            };

            ApplicationUser user = new()
            {
                Name = name,
                UserName = userName,
                Email = userName,
                NormalizedEmail = userName,
                EmailConfirmed = true
            };

            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).
                Returns(Task.FromResult(IdentityResult.Success));
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));
            _mockUserManager.Setup(y => y.FindByEmailAsync(userName)).ReturnsAsync(user);
            _mockUserManager.Setup(y => y.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("Si todo salio bien este es el token");


            var list = new List<IdentityRole>()
                {
                    new IdentityRole("admin"),
                    new IdentityRole("customer"),
                }.AsQueryable();

            _mockRoleManager.Setup(r => r.Roles).Returns(list);
            _mockRoleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));

            var repostitory = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object, _mapper, _mockRoleManager.Object, emailConfiguration);

            //Act
            var response = await repostitory.Registration(registrationRequestDto);
            //var currentResponse = response;

            //Assert
            Assert.True(response.User != null && response.Token != null);
            Assert.IsType<RegistrationResponseDto>(response);
        }

        [Theory]
        [InlineData("Yadira", "Juan4@gmail.com", "Admin1233!", "client")]
        [InlineData("Cris", "Juan5@gmail.com", "Admin1232!", "manager")]
        public async Task Registration_WhenRoleDoesntExists_ShouldThrowInvalidDataException(string name, string userName, string password, string role)
        {
            //Arrange
            ApplicationUser user = new()
            {
                UserName = userName,
                Name = name,
                Email = userName,
                NormalizedEmail = userName.ToUpper(),
                EmailConfirmed = true
            };

            RegistrationRequestDto registrationRequest = new()
            {
                UserName = userName,
                Name = name,
                Role = role,
                Password = password
            };

            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));

            _mockRoleManager.Setup(u => u.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var repository = new UserRepository(_mockConfiguration.Object,_mockUserManager.Object,_mapper,_mockRoleManager.Object,emailConfiguration);

            //Act
            var action = async () => await repository.Registration(registrationRequest);

            //Assert
            await Assert.ThrowsAnyAsync<InvalidDataException>(action);

        }


        [Theory]
        [InlineData("Yadira", "Juan4@gmail.com", "admin123", "client")]
        public async Task Registration_WhenPasswordFail_ReturnRegistrationResponseEqualNull(string name, string userName, string password, string role)
        {
            //Arrange
            RegistrationRequestDto registrationRequest = new()
            {
                UserName = userName,
                Name = name,
                Password = password,
                Role = role
            };

            _mockUserManager.Setup(u =>u.CreateAsync(It.IsAny<ApplicationUser>(),It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Failed()));

            var repository = new UserRepository(_mockConfiguration.Object,_mockUserManager.Object, _mapper, _mockRoleManager.Object, emailConfiguration);

            //Act
            var response = await repository.Registration(registrationRequest);

            //Assert
            Assert.True(response.User==null && response.Token == null);
        }

        [Theory]
        [InlineData("juan@gmail.com", true)]
        [InlineData("jose@gmail.com",false)]
        [InlineData("jennifer@gmail.com",true)]
        public async Task IsUnique_WHenSendUserNameToVerify_ShouldReturnTrueOrFalse(string userName, bool expectedStatus)
        {
            //Arrange
            ApplicationUser user = new()
            {
                Name = "juan",
                UserName = userName,
                Email = userName,
                NormalizedEmail = userName,
                EmailConfirmed = true
            };

            _mockUserManager.Setup(y => y.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(expectedStatus ? user : null);

            var repository = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object,_mapper, _mockRoleManager.Object,emailConfiguration);

            //Act
            var response = repository.IsUnique(userName);

            //Assert
            Assert.True(expectedStatus == !response);
        }


        [Theory]
        [InlineData("juan","juan@gmail.com","Admin123!")]
        public async Task Login_WhenSendCorrectLoginRequest_ReturnLoginResponse(string name,string username, string password)
        {
            //Arrange
            LoginRequestDto loginRequest = new()
            {
                UserName = username,
                Password = password
            };

            ApplicationUser user = new()
            {
                UserName = username,
                Name = name
            };

            List<string> roles = new()
            {
               "admin",
               "cusomer"
            };

            LoginResponseDto loginResponse = new()
            {
                User = _mapper.Map<UserDto>(user),
                Token = It.IsAny<string>(),
                Role = "admin",
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockUserManager.Setup(u => u.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(true);
            _mockUserManager.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);

            var repository = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object,_mapper, _mockRoleManager.Object, emailConfiguration);

            //Act
            var response = await repository.Login(loginRequest);

            //Assert
            Assert.NotNull(response.User);
            Assert.IsType<UserDto>(response.User);
            Assert.True(loginResponse.Role == response.Role);
        }


        [Theory]
        [InlineData("juan@gmail.com", "Admin123!", true, true,false)]
        [InlineData("juan@gmail.com", "Admin123!", false, true, false)]
        [InlineData("juan@gmail.com", "Admin123!", true, false, true)]
        //[InlineData("juan@gmail.com", "Admin123!", true, true, true)]
        public async Task Login_WhenUserDoesntExistsOrPasswordIncorrectOrEmailIsntConfirmed_ReturnLoginResponseNull(string username, string password, bool userExist, bool checkPassword, bool isEmailConfirmed)
        {
            //Arrange
            LoginRequestDto loginRequest = new()
            {
                UserName = username,
                Password = password
            };

            ApplicationUser user = new()
            {
                Name = "Juan",
                UserName = username,
                Email = username
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(userExist ? user : null);
            _mockUserManager.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(checkPassword);
            _mockUserManager.Setup(u => u.IsEmailConfirmedAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(isEmailConfirmed);
            var repository = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object, _mapper, _mockRoleManager.Object, emailConfiguration);

            //Act
            var response = await repository.Login(loginRequest);


            //Assert
            Assert.Null(response.User);
            Assert.Null(response.Token);
            Assert.Null(response.Role);

        }

        [Theory]
        [InlineData("Aqui va el token de confirmación de email","juan@gmail.com")]
        public async Task ConfirmEmail_WhenEmailIsConfirmed_ReturnTrue(string token, string userName)
        {
            //Arrange
            ApplicationUser user = new()
            {
                Name = "juan",
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.ConfirmEmailAsync(It.IsAny<ApplicationUser>(),It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            var repository = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object, _mapper, _mockRoleManager.Object, emailConfiguration);

            //Act
            var response = await repository.ConfirmEmail(token,userName);

            //Assert
            Assert.True(response);
        }

        [Theory]
        [InlineData("Aqui va el token de confirmación de email", "juan@gmail.com", true,false)]
        [InlineData("Aqui va el token de confirmación de email", "juan@gmail.com", false, true)]
        //[InlineData("Aqui va el token de confirmación de email", "juan@gmail.com", true, true)]
        public async Task ConfirmEmail_WhenUserDoesntExistsOrEmailIsntConfirmed_ReturnFalse(string token, string userName, bool userExist, bool emailConfirmed)
        {
            //Arrange
            ApplicationUser user = new()
            {
                Name = "juan",
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(userExist ? user : null);
            _mockUserManager.Setup(u => u.ConfirmEmailAsync(It.IsAny<ApplicationUser>(),It.IsAny<string>())).Returns(emailConfirmed ? Task.FromResult(IdentityResult.Success) : Task.FromResult(IdentityResult.Failed()));

            var repository = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object, _mapper, _mockRoleManager.Object, emailConfiguration);

            //Act
            var response = await repository.ConfirmEmail(token,userName);

            //Assert
            Assert.False(response);
        }


        [Theory]
        [InlineData("juan","juan@gmail.com")]
        [InlineData("yadi","yadi@gmail.com")]
        public async Task ForgetPassword_WhenSendCorrectRequest_ReturnResponseWithToken(string name,string userName)
        {
            //Arrange
            ApplicationUser user = new()
            {
                Name = name,
                UserName = userName,
                Email = userName,
                EmailConfirmed = true,
            };

            ForgetRequestDto forgetRequest = new()
            {
                UserName = userName
            };

            ForgetResponseDto forgetResponse = new()
            {
                User = _mapper.Map<UserDto>(user),
                Token = "Aqui va el token para cambiar la contraseña"
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("Aqui va el token para cambiar la contraseña");

            var repository = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object, _mapper, _mockRoleManager.Object, emailConfiguration);

            //Act
            var response = await repository.ForgetPassword(forgetRequest);

            //Assert
            Assert.NotNull(response.User);
            Assert.NotNull(response.Token);
            Assert.Equal(forgetResponse.User.Id,response.User.Id);
            Assert.Equal(forgetResponse.Token, response.Token);
        }

        [Fact]
        public async Task ForgetPassword_WhenUserDoesntExists_ReturnResponseNull()
        {
            //Arrange
            //ApplicationUser user = null;

            ForgetRequestDto forgetRequest = new()
            {
                UserName = "loquesea@gamil.com"
            };

            //ForgetResponseDto forgetResponse = null;

            //Es importante mencionar que si se desea retornar null no es neceseario configurar el mock ya que si no se
            //emplea el metodo Setup este siempre enviara null
            //_mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            var repository = new UserRepository(_mockConfiguration.Object,_mockUserManager.Object,_mapper, _mockRoleManager.Object, emailConfiguration);

            //Act
            var response = await repository.ForgetPassword(forgetRequest);

            //Assert
            Assert.Null(response.Token);
            Assert.Null(response.User);
        }


        [Theory]
        [InlineData("Juan","juan@gmail.com","Admin1231!","Aqui va el token para cambiar la contraseña", true)]
        [InlineData("Juan", "juan@gmail.com", "Admin1231!", null,false)]
        public async Task ResetPassword_WhenSendRequest_ShouldReturnTrueOrFalse(string name, string userName, string password, string token, bool statusExpected)
        {
            //Arrange
            ApplicationUser user = new()
            {
                Name = name,
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            ResetPasswordRequestDto resetPassword = new()
            {
                UserName = userName,
                Password = password,
                Token = token
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.ResetPasswordAsync(It.IsAny<ApplicationUser>(),It.IsAny<string>(), It.IsAny<string>())).Returns(!string.IsNullOrEmpty(token)? Task.FromResult(IdentityResult.Success): Task.FromResult(IdentityResult.Failed()));

            var repository = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object, _mapper, _mockRoleManager.Object,emailConfiguration);

            //Act
            var response = await repository.ResetPassword(resetPassword);

            //Assert
            Assert.True(statusExpected == response);
        }

        [Fact]
        public async Task ResetPassword_WhenUserDoesntExists_ReturnFalse()
        {
            //Arrange
            ResetPasswordRequestDto resetPassword = new()
            {
                UserName = "loquesea@gmail.com",
                Password = "loquesea",
                Token = "lo que sea"
            };

            var repository = new UserRepository(_mockConfiguration.Object, _mockUserManager.Object, _mapper, _mockRoleManager.Object,emailConfiguration);

            //Act
            var response = await repository.ResetPassword(resetPassword);

            //Assert
            Assert.False(response);
        }

    }
}



