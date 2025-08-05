namespace Backend_PowerGuardian.DTOs
{
    public class VentaDto
    {
        public string SKU { get; set; } = null!;
        public string Producto { get; set; } = null!;
        public decimal Precio { get; set; }
        public DateTime? FechaCompra { get; set; }
        public string Username { get; set; } = "—";
    }

}
