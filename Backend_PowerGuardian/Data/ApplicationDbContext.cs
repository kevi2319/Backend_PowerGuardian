using Backend_PowerGuardian.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Backend_PowerGuardian.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoUnidad> ProductoUnidades { get; set; }
        public DbSet<Manual> Manuales { get; set; }
        public DbSet<Dispositivo> Dispositivos { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<CompraProveedor> ComprasProveedor { get; set; }
        public DbSet<DetalleCompraProveedor> DetallesCompraProveedor { get; set; }
        public DbSet<MateriaPrima> MateriasPrimas { get; set; }
        public DbSet<RecetaProducto> RecetasProducto { get; set; }
        public DbSet<ProveedorProducto> ProveedorProductos { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<CotizacionDetalle> CotizacionDetalles { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación uno a uno entre Dispositivo y ProductoUnidad
            modelBuilder.Entity<Dispositivo>()
                .HasOne(d => d.ProductoUnidad)
                .WithOne()
                .HasForeignKey<Dispositivo>(d => d.ProductoUnidadId);

            // Relación muchos a uno entre Dispositivo y Usuario
            modelBuilder.Entity<Dispositivo>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Dispositivos)
                .HasForeignKey(d => d.UsuarioId);

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId);

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.ProductoUnidad)
                .WithMany()
                .HasForeignKey(r => r.ProductoUnidadId);

            // Configurar relación muchos a muchos entre Proveedor y Producto
            modelBuilder.Entity<ProveedorProducto>()
                .HasOne(pp => pp.Proveedor)
                .WithMany(p => p.ProveedorProductos)
                .HasForeignKey(pp => pp.ProveedorId);

            modelBuilder.Entity<ProveedorProducto>()
                .HasOne(pp => pp.Producto)
                .WithMany(p => p.ProveedorProductos)
                .HasForeignKey(pp => pp.ProductoId);

            // Índice único para evitar duplicados
            modelBuilder.Entity<ProveedorProducto>()
                .HasIndex(pp => new { pp.ProveedorId, pp.ProductoId })
                .IsUnique();
        }


        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<Backend_PowerGuardian.Models.Opinion> Opinion { get; set; } = default!;
    }
}
