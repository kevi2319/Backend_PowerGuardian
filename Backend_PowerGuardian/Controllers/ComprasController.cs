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

    [HttpPost("comprar")]
    public async Task<IActionResult> RegistrarCompra([FromBody] CompraDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        // Buscar unidad disponible del inventario
        var inventario = await _context.Inventarios
            .Where(i => i.ProductoId == dto.ProductoId && i.Estado == "disponible")
            .FirstOrDefaultAsync();

        if (inventario == null)
            return BadRequest("No hay stock disponible para este producto");

        // Generar SKU
        var sku = "PG-" + Guid.NewGuid().ToString("N")[..8].ToUpper();

        // Crear unidad
        var unidad = new ProductoUnidad
        {
            SKU = sku,
            Usado = false,
            ProductoId = dto.ProductoId
        };

        _context.ProductoUnidades.Add(unidad);

        // Marcar inventario como vendido y asignar al cliente (si ya existe)
        var user = await _userManager.FindByEmailAsync(email);
        inventario.Estado = "vendido";
        if (user != null)
        {
            inventario.UsuarioId = user.Id;
            await _correoService.EnviarCorreoAsociarDispositivo(email, sku);
        }
        else
        {
            await _correoService.EnviarCorreoRegistroConSku(email, sku);
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Compra registrada correctamente." });
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
