using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend_PowerGuardian.Models
{
    public class ProductoUnidad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } // Código único de la unidad

        public bool Usado { get; set; } = false; // Si ya fue registrado

        public DateTime? FechaCompra { get; set; }

        // Relación: cada unidad pertenece a un producto
        [ForeignKey("Producto")]
        public int ProductoId { get; set; }
        [JsonIgnore]
        public Producto Producto { get; set; }
    }
}
