using LineNatural.DTOs.LocalUser;
using LineNatural.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace LineNatural.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("Login")] 
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _userRepository.Login(loginRequestDto);
            if (loginResponse == null)
            {
                return NotFound();
            }

            return Ok(loginResponse);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            try
            {
                bool userNameExist = _userRepository.IsUnique(registrationRequestDto.UserName);
                if (!userNameExist)
                {
                    return BadRequest();
                }

                var response = await _userRepository.Registration(registrationRequestDto);
                return Ok(response);
            }
            catch (InvalidDataException)
            {
                return BadRequest();
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var isConfirm = await _userRepository.ConfirmEmail(token, email);
            if (isConfirm)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetRequestDto forgetRequestDto) 
        {
            var forgetResponse = await _userRepository.ForgetPassword(forgetRequestDto);
            if (forgetResponse != null)
            {
                return Ok(forgetResponse);
            }
            return BadRequest();
        }

        [HttpPost("ResetPasword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var resetSuccess = await _userRepository.ResetPassword(resetPasswordRequestDto);
            if (resetSuccess)
            {
                return Ok();
            }
            return BadRequest();
        }



    }
}
