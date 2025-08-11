namespace Backend_PowerGuardian.Models
{
    public class RecetaProducto
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public int MateriaPrimaId { get; set; }
        public MateriaPrima MateriaPrima { get; set; } = null!;

        public decimal Cantidad { get; set; }  // Cantidad utilizada en la receta
    }
}
