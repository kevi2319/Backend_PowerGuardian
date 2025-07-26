namespace Backend_PowerGuardian.DTOs
{
    public class PublicProductoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; }
        public int StockDisponible { get; set; }
    }
}