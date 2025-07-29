using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend_PowerGuardian.Controllers.Admin;

[Route("api/admin/productos")]
[Authorize(Roles = "Administrador")]
[ApiController]
public class ProductosAdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductosAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/admin/productos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetAll()
    {
        return await _context.Productos.ToListAsync();
    }

    // GET: api/admin/productos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetById(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        return producto == null ? NotFound() : Ok(producto);
    }

    // POST: api/admin/productos
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }

    // PUT: api/admin/productos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] Producto producto)
    {
        if (id != producto.Id) return BadRequest("IDs no coinciden");
        _context.Entry(producto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/admin/productos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();

        var tieneUnidades = await _context.ProductoUnidades.AnyAsync(u => u.ProductoId == id);
        if (tieneUnidades) return BadRequest("No se puede eliminar un producto con unidades asociadas.");

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("costos")]
    public async Task<IActionResult> ObtenerCostosProductos()
    {
        var productos = await _context.Productos
            .Include(p => p.RecetaProductos)
                .ThenInclude(r => r.MateriaPrima)
            .ToListAsync();

        var result = productos.Select(p => new
        {
            productoId = p.Id,
            nombre = p.Nombre,
            precioVenta = p.Precio,
            costoEstimado = p.RecetaProductos.Sum(r => r.Cantidad * r.MateriaPrima.CostoUnitario)
        });

        return Ok(result);
    }

}
