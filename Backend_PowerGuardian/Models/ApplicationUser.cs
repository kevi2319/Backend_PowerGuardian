using Microsoft.AspNetCore.Identity;

namespace Backend_PowerGuardian.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nombres { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? FechaNacimiento { get; set; }
        public string? Pais { get; set; }
        public string? Telefono { get; set; }

    }
}
