using Microsoft.AspNetCore.Identity;

namespace Backend_PowerGuardian.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
    }
}
