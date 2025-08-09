namespace Backend_PowerGuardian.DTOs
{
    public class CrearCotizacionDto
    {
        public string ClienteNombre { get; set; }
        public string ClienteCorreo { get; set; }
        public string Telefono { get; set; }
        public string Empresa { get; set; }
        public string Comentarios { get; set; }
        public List<DetalleCotizacionDto> Detalles { get; set; }
    }

    public class DetalleCotizacionDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

}
