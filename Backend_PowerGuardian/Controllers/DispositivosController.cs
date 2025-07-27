using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend_PowerGuardian.Services;
using Backend_PowerGuardian.DTOs;
using Backend_PowerGuardian.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_PowerGuardian.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DispositivosController : ControllerBase
    {
        private readonly IDispositivoService _service;
        private readonly ApplicationDbContext _context;

        public DispositivosController(IDispositivoService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet("mis")]
        [Authorize]
        public async Task<IActionResult> ObtenerMisDispositivos()
        {
            var usuarioId = User.FindFirst("id")?.Value;
            Console.WriteLine($"[DEBUG] Usuario autenticado: {usuarioId}");
            var dispositivos = await _service.ObtenerMisDispositivosAsync(usuarioId);
            return Ok(dispositivos);
        }


        [HttpPut("{id}/estado")]
        [Authorize]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto)
        {
            var usuarioId = User.FindFirst("id")?.Value;
            var ok = await _service.CambiarEstadoAsync(id, usuarioId, dto.Estado);
            return ok ? Ok() : NotFound();
        }

        [HttpPost("asociar")]
        [Authorize]
        public async Task<IActionResult> AsociarDispositivo([FromBody] AsociarDispositivoDto dto)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized("Usuario no válido.");

            var unidad = await _context.ProductoUnidades
                .FirstOrDefaultAsync(p => p.SKU == dto.SKU && !p.Usado);

            if (unidad == null)
                return BadRequest("El SKU ingresado es inválido o ya fue utilizado.");

            var dispositivo = await _service.CrearDesdeProductoUnidadAsync(unidad.Id, userId);
            if (dispositivo == null)
                return StatusCode(500, "No se pudo crear el dispositivo.");

            unidad.Usado = true;
            unidad.FechaCompra = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Dispositivo asociado correctamente." });
        }

        [HttpGet("{id}/consumo")]
        [Authorize]
        public IActionResult ObtenerHistorialConsumo(int id)
        {
            var userId = User.FindFirst("id")?.Value;
            var dispositivo = _context.Dispositivos.FirstOrDefault(d => d.Id == id && d.UsuarioId == userId);
            if (dispositivo == null) return NotFound("Dispositivo no encontrado");

            // Simular 30 días de consumo
            var random = new Random();
            var historial = Enumerable.Range(0, 30).Select(i => new ConsumoDto
            {
                Fecha = DateTime.Today.AddDays(-i),
                ConsumoKwH = Math.Round(random.NextDouble() * 1.5 + 0.1, 2)
            }).ToList();

            return Ok(historial.OrderBy(d => d.Fecha));
        }

    }
}
