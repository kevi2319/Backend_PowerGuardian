using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.DTOs;
using Backend_PowerGuardian.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_PowerGuardian.Services;

public class DispositivoService : IDispositivoService
{
    private readonly ApplicationDbContext _db;

    public DispositivoService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<DispositivoDto>> ObtenerMisDispositivosAsync(string usuarioId)
    {
        return await _db.Dispositivos
            .Where(d => d.UsuarioId == usuarioId)
            .Select(d => new DispositivoDto
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Estado = d.Estado,
                SKU = d.ProductoUnidad != null ? d.ProductoUnidad.SKU : "(sin SKU)",
                FechaRegistro = d.FechaRegistro
            }).ToListAsync();
    }

    public async Task<bool> CambiarEstadoAsync(int dispositivoId, string usuarioId, string nuevoEstado)
    {
        var dispositivo = await _db.Dispositivos.FirstOrDefaultAsync(d => d.Id == dispositivoId && d.UsuarioId == usuarioId);
        if (dispositivo == null) return false;

        dispositivo.Estado = nuevoEstado;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Dispositivo> CrearDesdeProductoUnidadAsync(int productoUnidadId, string usuarioId)
    {
        var unidad = await _db.ProductoUnidades.FirstOrDefaultAsync(p => p.Id == productoUnidadId && !p.Usado);
        if (unidad == null) return null;

        unidad.Usado = true;

        var dispositivo = new Dispositivo
        {
            ProductoUnidadId = unidad.Id,
            UsuarioId = usuarioId,
            Nombre = $"PowerGuardian {unidad.SKU}",
            Estado = "off"
        };

        _db.Dispositivos.Add(dispositivo);
        await _db.SaveChangesAsync();

        return dispositivo;
    }
}
