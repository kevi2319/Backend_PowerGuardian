namespace Backend_PowerGuardian.Models
{
public class Dispositivo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = "PowerGuardian";

    public string Estado { get; set; } = "off";

    public string UsuarioId { get; set; }
    public ApplicationUser Usuario { get; set; }

    public int ProductoUnidadId { get; set; }
    public ProductoUnidad ProductoUnidad { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}

}
