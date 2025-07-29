namespace Backend_PowerGuardian.DTOs
{
    public class ProveedorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Pais { get; set; }
        public bool Activo { get; set; }
    }

}
