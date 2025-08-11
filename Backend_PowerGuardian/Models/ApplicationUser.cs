using Microsoft.AspNetCore.Identity;

namespace Backend_PowerGuardian.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombres { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public DateTime?FechaNacimiento { get; set; }
        public string Pais { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navegación hacia los dispositivos del usuario
        public ICollection<Dispositivo> Dispositivos { get; set; }

    }
}
