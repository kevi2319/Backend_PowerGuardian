

namespace Backend_PowerGuardian.Models
{
    public class PzemData
    {
        public string DeviceId { get; set; }
        public float Voltaje { get; set; }
        public float Corriente { get; set; }
        public float Potencia { get; set; }
        public float FactorPotencia { get; set; } // Cambiado a FactorPotencia para coincidir con el JSON
        public float Frecuencia { get; set; }
        public float Energia { get; set; }
        public string Timestamp { get; set; } // O puedes convertirlo a DateTimeOffset o DateTime
    }
}
