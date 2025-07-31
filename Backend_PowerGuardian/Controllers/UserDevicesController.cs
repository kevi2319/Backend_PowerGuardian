using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Necesario para el atributo [Authorize]
using Microsoft.EntityFrameworkCore; // Necesario para Entity Framework Core
using System.Linq;
using System.Security.Claims; // Necesario para ClaimTypes
using System.Threading.Tasks;
using Backend_PowerGuardian.Data; // Tu DbContext
using Backend_PowerGuardian.Models; // Tus modelos (ProductoUnidad, PzemReading, etc.)
using System.Collections.Generic; // Para List

namespace Backend_PowerGuardian.Controllers
{
    //[Authorize] // Este controlador requiere que el usuario esté autenticado
    [Route("api/[controller]")] // La ruta base será /api/UserDevices
    [ApiController]
    public class UserDevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserDevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista de dispositivos (unidades de producto) vinculados al usuario autenticado.
        /// Este endpoint devolverá un 404 si no se encuentran dispositivos.
        /// </summary>
        /// <returns>Lista de dispositivos del usuario con detalles relevantes.</returns>
        [HttpGet("my-devices")] // La ruta completa será /api/UserDevices/my-devices
        public async Task<IActionResult> GetMyDevices()
        {
            // Obtener el ID del usuario autenticado desde los claims del token JWT
            // ClaimTypes.NameIdentifier suele ser el ID del usuario en Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Esto no debería ocurrir con [Authorize], pero es una buena verificación de seguridad
                return Unauthorized("No se pudo obtener el ID del usuario autenticado.");
            }

            // Buscar todas las ProductoUnidades que pertenecen a este usuario
            // y que tienen un DeviceId asignado (es decir, ya están vinculadas a un ESP32)
            var userDevices = await _context.ProductoUnidades
                .Where(pu => pu.UserId == userId && pu.DeviceId != null)
                .Include(pu => pu.Producto) // Incluir la información del Producto relacionado
                .Select(pu => new // Seleccionar solo los datos que la app móvil necesita
                {
                    pu.Id,
                    pu.SKU,
                    pu.DeviceId, // La MAC del ESP32
                    pu.FechaCompra,
                    ProductoDescripcion = pu.Producto != null ? pu.Producto.Descripcion : "N/A", // Descripción del producto
                    ProductoImagenUrl = pu.Producto != null ? pu.Producto.ImagenUrl : null // URL de la imagen del producto
                })
                .ToListAsync();

            if (!userDevices.Any())
            {
                // Si no se encuentran dispositivos, devolver 404 Not Found
                // Esto es lo que causaba el error 404 en tu app móvil si no tenías datos
                return NotFound("No se encontraron dispositivos vinculados a este usuario.");
            }

            return Ok(userDevices);
        }

        /// <summary>
        /// Obtiene la última lectura de energía para un dispositivo específico del usuario autenticado.
        /// </summary>
        /// <param name="deviceId">El ID del dispositivo (MAC del ESP32).</param>
        /// <returns>La última lectura de energía.</returns>
        [HttpGet("latest-reading/{deviceId}")] // La ruta completa será /api/UserDevices/latest-reading/{deviceId}
        public async Task<IActionResult> GetLatestReading(string deviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Verificar que el dispositivo pertenezca al usuario autenticado
            var isDeviceOwnedByUser = await _context.ProductoUnidades
                .AnyAsync(pu => pu.DeviceId == deviceId && pu.UserId == userId);

            if (!isDeviceOwnedByUser)
            {
                // Si el dispositivo no pertenece al usuario o no está vinculado, denegar acceso
                return Forbid("No tiene permiso para acceder a este dispositivo o el dispositivo no está vinculado a su cuenta.");
            }

            // 2. Obtener la última lectura para ese DeviceId
            var latestReading = await _context.PzemReadings
                .Where(pr => pr.DeviceId == deviceId)
                .OrderByDescending(pr => pr.Timestamp) // Ordenar por fecha descendente para obtener la más reciente
                .FirstOrDefaultAsync();

            if (latestReading == null)
            {
                return NotFound($"No se encontraron lecturas para el dispositivo con ID: {deviceId}.");
            }

            return Ok(latestReading);
        }

        /// <summary>
        /// Obtiene el historial de lecturas de energía para un dispositivo específico del usuario autenticado.
        /// </summary>
        /// <param name="deviceId">El ID del dispositivo (MAC del ESP32).</param>
        /// <param name="startDate">Fecha de inicio (opcional, formato YYYY-MM-DD).</param>
        /// <param name="endDate">Fecha de fin (opcional, formato YYYY-MM-DD).</param>
        /// <param name="limit">Número máximo de lecturas a devolver (por defecto 100).</param>
        /// <returns>Lista de lecturas de energía.</returns>
        [HttpGet("history/{deviceId}")] // La ruta completa será /api/UserDevices/history/{deviceId}
        public async Task<IActionResult> GetHistory(string deviceId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int limit = 100)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Verificar que el dispositivo pertenezca al usuario autenticado
            var isDeviceOwnedByUser = await _context.ProductoUnidades
                .AnyAsync(pu => pu.DeviceId == deviceId && pu.UserId == userId);

            if (!isDeviceOwnedByUser)
            {
                return Forbid("No tiene permiso para acceder a este dispositivo o el dispositivo no está vinculado a su cuenta.");
            }

            // 2. Construir la consulta de historial
            var query = _context.PzemReadings.Where(pr => pr.DeviceId == deviceId);

            if (startDate.HasValue)
            {
                // Comparar con la fecha y hora UTC para asegurar consistencia
                query = query.Where(pr => pr.Timestamp >= startDate.Value.ToUniversalTime());
            }
            if (endDate.HasValue)
            {
                // Sumar un día para incluir todo el día final, luego convertir a UTC
                query = query.Where(pr => pr.Timestamp <= endDate.Value.AddDays(1).ToUniversalTime());
            }

            var history = await query
                .OrderByDescending(pr => pr.Timestamp) // Ordenar por fecha descendente para los más recientes primero
                .Take(limit) // Limitar el número de resultados
                .ToListAsync();

            if (!history.Any())
            {
                return NotFound($"No se encontraron lecturas históricas para el dispositivo con ID: {deviceId} en el rango especificado.");
            }

            return Ok(history);
        }
    }
}
