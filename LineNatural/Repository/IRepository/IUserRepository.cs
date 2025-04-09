using LineNatural.DTOs.LocalUser;
using LineNatural.Entities;
using LineNatural.Utility;

namespace LineNatural.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUnique(string UserName);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

        Task<RegistrationResponseDto> Registration(RegistrationRequestDto registrationRequestDto);

        Task<ForgetResponseDto> ForgetPassword(ForgetRequestDto forgetRequestDto);

        Task<bool> ResetPassword(ResetPasswordRequestDto resetPasswordRequestDto);

        Task<bool> ConfirmEmail(string token, string email);

        void SendEmail(Message message);


    }
}
