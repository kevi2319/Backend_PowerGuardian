using System.ComponentModel.DataAnnotations;

namespace Backend_PowerGuardian.Models
{
    public class Contacto
    {
        public int ContactoId { get; set; }
        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }
        [MaxLength(500)]
        public string Mensaje { get; set; }

        public int ProductoId { get; set; }

        public Producto? Producto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

    }
}
