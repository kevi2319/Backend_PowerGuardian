namespace Backend_PowerGuardian.DTOs
{
    public class InventarioDto
    {
        public int Id { get; set; }
        public string? Sku { get; set; }
        public string? ClienteUsuario { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public string Estado { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string? Notas { get; set; }
    }

}
