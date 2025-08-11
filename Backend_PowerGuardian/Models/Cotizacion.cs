using System.ComponentModel.DataAnnotations;

namespace Backend_PowerGuardian.Models
{
    public class Cotizacion
    {
        public int Id { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteCorreo { get; set; }
        public string Telefono { get; set; }
        public string Empresa { get; set; }
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public string Comentarios { get; set; }
        public decimal TotalEstimado { get; set; }
        public List<CotizacionDetalle> Detalles { get; set; }
    }
}
