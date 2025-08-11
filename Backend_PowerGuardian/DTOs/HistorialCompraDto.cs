public class HistorialCompraDto
{
    public int UnidadId { get; set; }
    public string SKU { get; set; } = null!;
    public DateTime? FechaCompra { get; set; }
    public string ProductoNombre { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public bool Usado { get; set; }
    public bool TieneResena { get; set; }
    public int? Calificacion { get; set; }
    public string? Comentario { get; set; }

}
