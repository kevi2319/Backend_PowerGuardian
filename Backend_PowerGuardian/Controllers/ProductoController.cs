using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.DTOs;

namespace Backend_PowerGuardian.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .Include(p => p.Unidades)
                .ToListAsync();
        }

        [HttpGet("publico")]
        public async Task<ActionResult<IEnumerable<PublicProductoDto>>> GetProductosPublicos()
        {
            var productos = await _context.Productos
                .Select(p => new PublicProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    ImagenUrl = p.ImagenUrl,
                    StockDisponible = _context.Inventarios.Count(i => i.ProductoId == p.Id && i.Estado == "disponible")
                })
                .ToListAsync();

            return Ok(productos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Unidades)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound();

            return producto;
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<int>> GetStockDisponible(int id)
        {
            var count = await _context.ProductoUnidades
                .CountAsync(u => u.ProductoId == id && !u.Usado);

            return Ok(count);
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
                return BadRequest();

            _context.Entry(producto).State = EntityState.Modified;
            _context.Entry(producto).Collection(p => p.Unidades).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Unidades)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/agregar-stock")]
        public async Task<IActionResult> AgregarStock(int id, [FromBody] int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado");

            var nuevasUnidades = new List<ProductoUnidad>();
            for (int i = 0; i < cantidad; i++)
            {
                nuevasUnidades.Add(new ProductoUnidad
                {
                    SKU = Guid.NewGuid().ToString(),
                    ProductoId = id,
                    Usado = false
                });
            }

            await _context.ProductoUnidades.AddRangeAsync(nuevasUnidades);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Stock agregado", cantidad });
        }

        [HttpPost("subir-imagen")]
        public async Task<IActionResult> SubirImagen(IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0)
                return BadRequest("No se ha proporcionado ninguna imagen");

            // Validar tipo de archivo
            var tiposPermitidos = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            if (!tiposPermitidos.Contains(imagen.ContentType.ToLower()))
                return BadRequest("Tipo de archivo no válido. Solo se permiten JPG, PNG y GIF");

            // Validar tamaño (máximo 5MB)
            if (imagen.Length > 5 * 1024 * 1024)
                return BadRequest("El archivo es demasiado grande. Máximo 5MB");

            try
            {
                // Crear directorio si no existe
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "productos");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // Generar nombre único para el archivo
                var extension = Path.GetExtension(imagen.FileName);
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var rutaCompleta = Path.Combine(uploadsPath, nombreArchivo);

                // Guardar archivo
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                // Devolver URL relativa
                var imagenUrl = $"/uploads/productos/{nombreArchivo}";
                return Ok(new { imagenUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir imagen: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al subir la imagen");
            }
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.DTOs;

namespace Backend_PowerGuardian.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .Include(p => p.Unidades)
                .ToListAsync();
        }

        [HttpGet("publico")]
        public async Task<ActionResult<IEnumerable<PublicProductoDto>>> GetProductosPublicos()
        {
            var productos = await _context.Productos
                .Include(p => p.Unidades)
                .Select(p => new PublicProductoDto
                {
                    Id = p.Id,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    ImagenUrl = p.ImagenUrl,
                    StockDisponible = p.Unidades.Count(u => !u.Usado)
                })
                .ToListAsync();

            return Ok(productos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Unidades)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound();

            return producto;
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<int>> GetStockDisponible(int id)
        {
            var count = await _context.ProductoUnidades
                .CountAsync(u => u.ProductoId == id && !u.Usado);

            return Ok(count);
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
                return BadRequest();

            _context.Entry(producto).State = EntityState.Modified;
            _context.Entry(producto).Collection(p => p.Unidades).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Unidades)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/agregar-stock")]
        public async Task<IActionResult> AgregarStock(int id, [FromBody] int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado");

            var nuevasUnidades = new List<ProductoUnidad>();
            for (int i = 0; i < cantidad; i++)
            {
                nuevasUnidades.Add(new ProductoUnidad
                {
                    SKU = Guid.NewGuid().ToString(),
                    ProductoId = id,
                    Usado = false,
                    DeviceId = null,
                    UserId = null
                });
            }

            await _context.ProductoUnidades.AddRangeAsync(nuevasUnidades);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Stock agregado", cantidad });
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
