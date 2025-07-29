namespace Backend_PowerGuardian.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Pais { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<CompraProveedor> Compras { get; set; } = new List<CompraProveedor>();

    }

}
