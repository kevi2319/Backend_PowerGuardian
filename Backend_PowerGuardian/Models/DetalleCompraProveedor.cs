namespace Backend_PowerGuardian.Models
{
    public class DetalleCompraProveedor
    {
        public int Id { get; set; }
        public int CompraProveedorId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }

        public CompraProveedor CompraProveedor { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }

}
