using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public Producto Producto { get; set; }

        // Para almacenar la Mac del ESP32 que esta vinculada a la undiad
        [MaxLength(25)]
        public string? DeviceId { get; set; }

        public string? UserId { get; set; }
    }
}
