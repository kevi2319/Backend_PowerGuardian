using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

// productoUnidad representa una unidad q ya fue vendida, con SKU asignado y asociada al cliente. pude ponerle un nombre más descriptivo como ProductoVendido pero cuando me di cuenta de eso ya era muy tarde y ya estaba referenciado en muchos lugares y ps me dio hueva cambiarlo xd
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

        public Dispositivo? Dispositivo { get; set; }
    }
}
