namespace Backend_PowerGuardian.DTOs
{
    public class PublicProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; }
        public int StockDisponible { get; set; }
    }
}