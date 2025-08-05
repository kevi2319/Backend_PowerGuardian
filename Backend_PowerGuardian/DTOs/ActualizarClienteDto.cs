namespace Backend_PowerGuardian.DTOs
{
    public class ActualizarClienteDto
    {
        public string Nombres { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }
}
