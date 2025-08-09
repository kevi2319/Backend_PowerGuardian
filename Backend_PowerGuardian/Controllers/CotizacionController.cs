using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend_PowerGuardian.Services;
using Backend_PowerGuardian.DTOs;
using Backend_PowerGuardian.Data;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Models;

namespace Backend_PowerGuardian.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICosteoService _costeoService;
        private readonly ICorreoService _correoService;

        public CotizacionController(ApplicationDbContext context, ICosteoService costeoService, ICorreoService correoService)
        {
            _context = context;
            _costeoService = costeoService;
            _correoService = correoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearCotizacion([FromBody] CrearCotizacionDto dto)
        {
            var cotizacion = new Models.Cotizacion
            {
                ClienteNombre = dto.ClienteNombre,
                ClienteCorreo = dto.ClienteCorreo,
                Telefono = dto.Telefono,
                Empresa = dto.Empresa,
                Comentarios = dto.Comentarios,
                Detalles = new List<CotizacionDetalle>()
            };

            decimal total = 0;

            foreach (var detalle in dto.Detalles)
            {
                var costoUnitario = await _costeoService.CalcularCostoProductoAsync(detalle.ProductoId);
                var subtotal = costoUnitario * detalle.Cantidad;
                total += subtotal;

                cotizacion.Detalles.Add(new CotizacionDetalle
                {
                    Producto = $"Producto ID: {detalle.ProductoId}",
                    Cantidad = detalle.Cantidad,
                    CostoUnitario = costoUnitario
                });
            }

            cotizacion.TotalEstimado = total;

            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            // Enviar correo al cliente
            var cuerpo = GenerarCorreoHtml(cotizacion);
            await _correoService.EnviarCorreo(cotizacion.ClienteCorreo, "Tu Cotización PowerGuardian", cuerpo);

            return Ok(new { mensaje = "Cotización generada y enviada con éxito." });
        }

        private string GenerarCorreoHtml(Cotizacion cot)
        {
            var html = $@"
                <h2>Cotización PowerGuardian</h2>
                <p>Gracias por tu solicitud, {cot.ClienteNombre}.</p>
                <ul>";
            foreach (var d in cot.Detalles)
            {
                html += $"<li>{d.Producto}: {d.Cantidad} x ${d.CostoUnitario} = ${d.Cantidad * d.CostoUnitario}</li>";
            }
            html += $@"</ul>
                <p><strong>Total estimado: ${cot.TotalEstimado}</strong></p>";
            return html;
        }
    }

}
