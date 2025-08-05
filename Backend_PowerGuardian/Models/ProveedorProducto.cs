using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_PowerGuardian.Models
{
    public class ProveedorProducto
    {
        [Key]
        public int Id { get; set; }

        public int ProveedorId { get; set; }
        [ForeignKey("ProveedorId")]
        public Proveedor Proveedor { get; set; } = null!;

        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; } = null!;

        public decimal? PrecioProveedor { get; set; } // Precio al que el proveedor vende este producto
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
