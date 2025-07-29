using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_PowerGuardian.Models;

public class Inventario
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductoId { get; set; }

    [ForeignKey("ProductoId")]
    public Producto Producto { get; set; }
    public string? UsuarioId { get; set; }

    public string Estado { get; set; } = "disponible";
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public string? Notas { get; set; }
}
