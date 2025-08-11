namespace Backend_PowerGuardian.DTOs
{
    public class ContactoDto
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Mensaje { get; set; }
        public string Asunto { get; set; }

        // Solo requeridos si es cotización
        public int? ProductoId { get; set; }
        public int? Cantidad { get; set; }
    }
}
