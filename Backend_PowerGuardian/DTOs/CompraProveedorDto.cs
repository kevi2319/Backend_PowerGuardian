namespace Backend_PowerGuardian.DTOs
{
    public class CompraProveedorDto
    {
        public int ProveedorId { get; set; }
        public string? Notas { get; set; }
        public List<DetalleCompraDto> Detalles { get; set; } = new();
    }

    public class DetalleCompraDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

}
