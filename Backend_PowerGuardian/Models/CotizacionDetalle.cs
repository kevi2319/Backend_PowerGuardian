using System.ComponentModel.DataAnnotations;

namespace Backend_PowerGuardian.Models
{
    public class CotizacionDetalle
    {
        public int Id { get; set; }
        public int CotizacionId { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }

        public Cotizacion Cotizacion { get; set; }
    }
}
