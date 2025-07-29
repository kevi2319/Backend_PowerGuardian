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
        }

    }
}
