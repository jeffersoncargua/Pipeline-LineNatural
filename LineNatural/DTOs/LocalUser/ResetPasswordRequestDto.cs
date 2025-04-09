using System.ComponentModel.DataAnnotations;

namespace LineNatural.DTOs.LocalUser
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
