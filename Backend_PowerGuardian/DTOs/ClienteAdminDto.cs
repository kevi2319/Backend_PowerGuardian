namespace Backend_PowerGuardian.DTOs
{
    public class ClienteAdminDto
    {
        public string Id { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public DateTime FechaRegistro { get; set; }
        public int Dispositivos { get; set; }
        public bool Activo { get; set; }
    }

}
