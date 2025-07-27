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
}
