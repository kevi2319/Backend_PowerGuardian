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

        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<Backend_PowerGuardian.Models.Opinion> Opinion { get; set; } = default!;
    }
}
