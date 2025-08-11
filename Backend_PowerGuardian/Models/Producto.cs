using System.ComponentModel.DataAnnotations;

namespace Backend_PowerGuardian.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Nombre { get; set; } = null!;

        [MaxLength(500)]
        public string Descripcion { get; set; } = null!;

        public decimal Precio { get; set; }

        [MaxLength(200)]
        public string ImagenUrl { get; set; } = null!;

        // Relación: Un producto tiene muchas unidades
        public ICollection<ProductoUnidad> Unidades { get; set; } = new List<ProductoUnidad>();

        // Relación con receta (materias primas asociadas)
        public ICollection<RecetaProducto> RecetaProductos { get; set; } = new List<RecetaProducto>();

        // Relación muchos a muchos con Proveedores
        public ICollection<ProveedorProducto> ProveedorProductos { get; set; } = new List<ProveedorProducto>();
    }
}
