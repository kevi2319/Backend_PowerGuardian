using System.ComponentModel.DataAnnotations;

namespace Backend_PowerGuardian.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        public decimal Precio { get; set; }

        [MaxLength(200)]
        public string ImagenUrl { get; set; }

        // Relación: Un producto tiene muchas unidades
        public ICollection<ProductoUnidad> Unidades { get; set; }
    }
}
