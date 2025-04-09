using System.ComponentModel.DataAnnotations;

namespace LineNatural.DTOs.LocalUser
{
    public class RegistrationRequestDto
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
