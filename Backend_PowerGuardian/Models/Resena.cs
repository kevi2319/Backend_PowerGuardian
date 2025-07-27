using Microsoft.AspNetCore.Identity;

namespace Backend_PowerGuardian.Models
{
public class Resena
{
    public int Id { get; set; }
    public int ProductoUnidadId { get; set; }
    public string UsuarioId { get; set; } = null!;
    public int Calificacion { get; set; } // 1 a 5
    public string Comentario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public ApplicationUser Usuario { get; set; } = null!;
    public ProductoUnidad ProductoUnidad { get; set; } = null!;
}

}
