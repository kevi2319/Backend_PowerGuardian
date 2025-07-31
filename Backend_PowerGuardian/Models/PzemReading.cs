using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_PowerGuardian.Models
{
    public class PzemReading
    {
        [Key] // Marca esta propiedad como la clave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indica que la BD generará el valor (autoincremental)
        public int Id { get; set; } // Identificador único para cada lectura

        public string DeviceId { get; set; } // El ID del ESP32
        public float Voltaje { get; set; }
        public float Corriente { get; set; }
        public float Potencia { get; set; }
        public float FactorPotencia { get; set; }
        public float Frecuencia { get; set; }
        public float Energia { get; set; }
        public DateTimeOffset Timestamp { get; set; } // Fecha y hora de la lectura (con información de zona horaria)
    }
}
