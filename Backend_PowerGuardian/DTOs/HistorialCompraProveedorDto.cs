namespace Backend_PowerGuardian.DTOs
{
    public class HistorialCompraProveedorDto
    {
        public string ProveedorNombre { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string ProductoNombre { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}
