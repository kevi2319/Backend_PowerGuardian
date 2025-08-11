namespace Backend_PowerGuardian.Models
{
    public class MateriaPrima
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string UnidadMedida { get; set; } = null!;
        public decimal CostoUnitario { get; set; }
        public bool Activo { get; set; } = true;
    }
}
