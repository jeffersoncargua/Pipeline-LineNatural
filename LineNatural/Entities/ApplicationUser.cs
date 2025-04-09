using Microsoft.AspNetCore.Identity;

namespace LineNatural.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
