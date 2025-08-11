namespace Backend_PowerGuardian.DTOs
{
    public class AgregarInventarioBulkDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? Notas { get; set; }
    }

}
