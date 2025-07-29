namespace Backend_PowerGuardian.Models
{
    public class CompraProveedor
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public DateTime FechaCompra { get; set; } = DateTime.UtcNow;
        public string? Notas { get; set; }

        public Proveedor Proveedor { get; set; } = null!;
        public ICollection<DetalleCompraProveedor> Detalles { get; set; } = new List<DetalleCompraProveedor>();
    }

}
