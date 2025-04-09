using AutoMapper;
using LineNatural.Context;
using LineNatural.DTOs.LocalUser;
using LineNatural.Entities;
using LineNatural.Repository.IRepository;
using LineNatural.Utility;
using MailKit.Net.Smtp; //Se debe instalar el paquete MailKit para poder utilizar SMTP para enviar emails.
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace LineNatural.Repository
{
    public class UserRepository : IUserRepository
    {
        //private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly EmailConfiguration _emailConfiguration;
        private string secretKey;
        //public UserRepository(IConfiguration configuration,
        //    UserManager<ApplicationUser> userManager, IMapper mapper,
        //    RoleManager<IdentityRole> roleManager, EmailConfiguration emailConfiguration)
        public UserRepository(IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper,
            RoleManager<IdentityRole> roleManager, EmailConfiguration emailConfiguration)
        {
            //_db = db;
            _userManager = userManager;
            _mapper = mapper;
            this.secretKey = configuration.GetValue<string>("APISettings:Secret");
            _roleManager = roleManager;
            _emailConfiguration = emailConfiguration;
            
        }

        public async Task<bool> ConfirmEmail(string token, string email)
        {
            var user =await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result =await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task<ForgetResponseDto> ForgetPassword(ForgetRequestDto forgetRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(forgetRequestDto.UserName);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgetPasswordLink = $"/api/User/?token={token}&email={user.Email}";

                var message = new Message(new string[] { user.Email }, "Reset Password", $"Link for change password {forgetPasswordLink}");
                SendEmail(message);

                return new ForgetResponseDto
                {
                    User = _mapper.Map<UserDto>(user),
                    Token = token
                };
            }
            return new ForgetResponseDto()
            {
                User = null,
                Token = null
            };
        }

        public bool IsUnique(string UserName)
        {
            //var user = _db.ApplicationUserTbl.FirstOrDefault(u => u.UserName == UserName);
            var user = _userManager.FindByEmailAsync(UserName);
            var result = user.Result;
            if (result != null)
            {
                return false;
            }

            return true;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.UserName);
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            var emailConfirm = await _userManager.IsEmailConfirmedAsync(user);

            if (user == null || isValid == false || !emailConfirm)
            {
                return new LoginResponseDto
                {
                    Token = null,
                    User = null,
                    Role = null
                };
            }            
            
            var roles = await _userManager.GetRolesAsync(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDto loginResponseDto = new()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDto>(user),
                Role = roles.FirstOrDefault()
            };
            return loginResponseDto;

        }

        public async Task<RegistrationResponseDto> Registration(RegistrationRequestDto registrationRequestDto)
        {

            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.UserName,
                Email = registrationRequestDto.UserName,
                NormalizedEmail = registrationRequestDto.UserName.ToUpper(),
                Name = registrationRequestDto.Name,
                EmailConfirmed = true,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    //En caso de que no exista un rol en la base de datos, se genera dos roles para asiganar
                    //a los usuarios
                    var rolesExist = await _roleManager.RoleExistsAsync(registrationRequestDto.Role); //Si existe retorna un true caso contrario false
                    //if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    if (!rolesExist)
                    {
                        //await _roleManager.CreateAsync(new IdentityRole("admin"));
                        //await _roleManager.CreateAsync(new IdentityRole("customer"));
                        throw new InvalidDataException();
                    }

                    await _userManager.AddToRoleAsync(user, registrationRequestDto.Role);

                    var tokenEmail = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //Se genera el link que va a tener el token y el email para utilizarlo en el frontEnd para luego ocuparlo en el API
                    //De momento vamos a intentar probarlo ingresando al API directamente
                    var confirmationLink = $"/api/User/ConfirmEmail?token={tokenEmail}&email={user.Email}";

                    var message = new Message(new string[] {user.Email},"Email Confirmation",confirmationLink);

                    SendEmail(message); //Permite enviar el token para confirmar el email al email de usuario que se ha registrado

                    //var userToReturn = _db.ApplicationUserTbl.
                    //    FirstOrDefaultAsync(u => u.UserName == registrationRequestDto.UserName);
                    var userToReturn = await _userManager.FindByEmailAsync(registrationRequestDto.UserName);
                    
                    return new RegistrationResponseDto()
                    {
                        User = _mapper.Map<UserDto>(userToReturn),
                        Token = tokenEmail
                    };
                }
                else
                {
                    return new RegistrationResponseDto()
                    {
                        User = null,
                        Token = null
                    };
                }
            }
            catch (InvalidDataException)
            {

                throw new InvalidDataException();
            }
            
            
        }

        public async Task<bool> ResetPassword(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequestDto.UserName);
            if (user != null)
            {
                var resetResult = await _userManager.ResetPasswordAsync(user,resetPasswordRequestDto.Password, resetPasswordRequestDto.Token);
                if (!resetResult.Succeeded)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Notification", _emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content};

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfiguration.SmtpServer,_emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);

                client.Send(mailMessage);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}

