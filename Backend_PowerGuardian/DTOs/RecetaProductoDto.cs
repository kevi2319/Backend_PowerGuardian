namespace Backend_PowerGuardian.DTOs
{
    public class RecetaProductoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = null!;
        public int MateriaPrimaId { get; set; }
        public string MateriaPrimaNombre { get; set; } = null!;
        public string UnidadMedida { get; set; } = null!;
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Subtotal => CostoUnitario * Cantidad;
    }
}
