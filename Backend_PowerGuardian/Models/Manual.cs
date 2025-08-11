namespace Backend_PowerGuardian.Models
{
    public class Manual
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string UrlArchivo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public bool Eliminado { get; set; } = false;
    }
}
