using Backend_PowerGuardian.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend_PowerGuardian.Seed
{
    public static class ProductoSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var context = new Data.ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<Data.ApplicationDbContext>>()))
            {
                if (context.Productos.Any())
                    return; // Ya hay productos

                var productos = new List<Producto>
                {
                    new Producto
                    {
                        Descripcion = "PowerGuardian 1000W - Protección eléctrica avanzada.",
                        Precio = 1200.00m,
                        ImagenUrl = "https://via.placeholder.com/200x200.png?text=PowerGuardian+1000W",
                        Unidades = new List<ProductoUnidad>
                        {
                            new ProductoUnidad { SKU = "PG1000-0001" },
                            new ProductoUnidad { SKU = "PG1000-0002" },
                            new ProductoUnidad { SKU = "PG1000-0003" }
                        }
                    },
                    new Producto
                    {
                        Descripcion = "PowerGuardian 2000W - Protección premium.",
                        Precio = 1800.00m,
                        ImagenUrl = "https://via.placeholder.com/200x200.png?text=PowerGuardian+2000W",
                        Unidades = new List<ProductoUnidad>
                        {
                            new ProductoUnidad { SKU = "PG2000-0001" },
                            new ProductoUnidad { SKU = "PG2000-0002" }
                        }
                    }
                };

                context.Productos.AddRange(productos);
                context.SaveChanges();
            }
        }
    }
}
