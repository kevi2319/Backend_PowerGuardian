using Microsoft.AspNetCore.Http;

namespace Backend_PowerGuardian.DTOs
{
    public class ManualUploadDto
    {
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public required IFormFile Archivo { get; set; }
    }
}
