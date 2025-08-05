namespace Backend_PowerGuardian.DTOs
{
    public class ProveedorProductoDto
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = null!;
        public decimal? PrecioProveedor { get; set; }
        public bool Activo { get; set; }
    }

    public class AsignarProductoProveedorDto
    {
        public int ProveedorId { get; set; }
        public int ProductoId { get; set; }
        public decimal? PrecioProveedor { get; set; }
    }
}
