using System.ComponentModel.DataAnnotations;

namespace Backend_PowerGuardian.Models
{
    public class Opinion
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string? Nombre { get; set; }

        [Required, MaxLength(800)]
        public string? Mensaje { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
