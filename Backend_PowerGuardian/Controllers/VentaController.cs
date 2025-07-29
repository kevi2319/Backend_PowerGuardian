using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.Models;

namespace Backend_PowerGuardian.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("comprar")]
        public async Task<IActionResult> ComprarProducto([FromQuery] int productoId, [FromQuery] int cantidad)
        {
            if (cantidad <= 0)
            {
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a 0." });
            }

            var productoExiste = await _context.Productos.AnyAsync(p => p.Id == productoId);
            if (!productoExiste)
            {
                return NotFound(new { mensaje = "Producto no encontrado." });
            }

            var disponibles = await _context.ProductoUnidades
                .Where(u => u.ProductoId == productoId && !u.Usado)
                .Take(cantidad)
                .ToListAsync();

            if (disponibles.Count < cantidad)
            {
                return BadRequest(new
                {
                    mensaje = $"Stock insuficiente. Solo hay {disponibles.Count} disponibles."
                });
            }

            foreach (var unidad in disponibles)
            {
                unidad.Usado = true;
                unidad.FechaCompra = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Compra realizada",
                unidades = disponibles.Select(u => u.SKU).ToList()
            });
        }
    }
}
