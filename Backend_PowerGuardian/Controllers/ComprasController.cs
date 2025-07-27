using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.Services;
using Backend_PowerGuardian.DTOs;
using Backend_PowerGuardian.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICorreoService _correoService;
    private readonly ILogger<ComprasController> _logger;

    public ComprasController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ICorreoService correoService,
        ILogger<ComprasController> logger)
    {
        _context = context;
        _userManager = userManager;
        _correoService = correoService;
        _logger = logger;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarCompra([FromBody] CompraDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        // Generar SKU
        var sku = "PG-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

        var unidad = new ProductoUnidad
        {
            SKU = sku,
            Usado = false,
            ProductoId = dto.ProductoId
        };

        _context.ProductoUnidades.Add(unidad);
        await _context.SaveChangesAsync();

        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
            await _correoService.EnviarCorreoAsociarDispositivo(email, sku);
        else
            await _correoService.EnviarCorreoRegistroConSku(email, sku);

        return Ok(new { message = "Compra registrada y correo enviado." });
    }

    [Authorize]
    [HttpGet("mis")]
    public async Task<IActionResult> ObtenerHistorial()
    {
        var userId = User.FindFirst("id")?.Value
           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("No se pudo obtener el ID del usuario.");

        var unidades = await _context.ProductoUnidades
            .Where(pu => _context.Dispositivos.Any(d => d.ProductoUnidadId == pu.Id && d.UsuarioId == userId)
                    || pu.Usado && pu.Dispositivo!.UsuarioId == userId) // si fue usado
            .Include(pu => pu.Producto)
            .ToListAsync();

        var historial = unidades.Select(u => {
            var resena = _context.Resenas.FirstOrDefault(r => r.ProductoUnidadId == u.Id);
            return new HistorialCompraDto
            {
                UnidadId = u.Id,
                SKU = u.SKU,
                FechaCompra = u.FechaCompra,
                ProductoNombre = u.Producto.Nombre,
                ImagenUrl = u.Producto.ImagenUrl,
                Usado = u.Usado,
                TieneResena = resena != null,
                Calificacion = resena?.Calificacion,
                Comentario = resena?.Comentario
            };
        }).ToList();


        return Ok(historial);
    }

}
