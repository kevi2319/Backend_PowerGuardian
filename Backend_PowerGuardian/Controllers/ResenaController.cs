using Microsoft.AspNetCore.Mvc;
using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.DTOs;
using Backend_PowerGuardian.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ResenaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ResenaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Crear([FromBody] CrearResenaDto dto)
    {
        var userId = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var unidad = await _context.ProductoUnidades.FindAsync(dto.ProductoUnidadId);
        if (unidad == null || !unidad.Usado)
            return BadRequest("La unidad no existe o aún no ha sido usada");

        var yaExiste = await _context.Resenas.AnyAsync(r =>
            r.ProductoUnidadId == dto.ProductoUnidadId && r.UsuarioId == userId);

        if (yaExiste)
            return BadRequest("Ya has enviado una reseña para esta unidad");

        var resena = new Resena
        {
            UsuarioId = userId,
            ProductoUnidadId = dto.ProductoUnidadId,
            Calificacion = dto.Calificacion,
            Comentario = dto.Comentario
        };

        _context.Resenas.Add(resena);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Reseña guardada correctamente" });
    }

    [HttpGet("resenas")]
    public async Task<IActionResult> ObtenerResenas()
    {
        var resenas = await _context.Resenas
            .Include(r => r.ProductoUnidad)
                .ThenInclude(pu => pu.Producto)
            .Include(r => r.Usuario)
            .Select(r => new ResenaAdminDto
            {
                Id = r.Id,
                ProductoNombre = r.ProductoUnidad.Producto.Nombre,
                SKU = r.ProductoUnidad.SKU,
                Cliente = r.Usuario.Nombres + " " + r.Usuario.ApellidoPaterno,
                Calificacion = r.Calificacion,
                Comentario = r.Comentario,
                Fecha = r.Fecha
            })
            .ToListAsync();

        return Ok(resenas);
    }
}
