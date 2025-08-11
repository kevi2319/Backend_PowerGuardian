namespace Backend_PowerGuardian.DTOs
{
    public class ResenaAdminDto
    {
        public int Id { get; set; }
        public string ProductoNombre { get; set; }
        public string SKU { get; set; }
        public string Cliente { get; set; }
        public int Calificacion { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }

}
