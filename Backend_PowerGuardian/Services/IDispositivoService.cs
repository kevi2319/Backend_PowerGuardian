namespace Backend_PowerGuardian.Services
{
    using Backend_PowerGuardian.DTOs;
    using Backend_PowerGuardian.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDispositivoService
    {
        Task<List<DispositivoDto>> ObtenerMisDispositivosAsync(string usuarioId);
        Task<bool> CambiarEstadoAsync(int dispositivoId, string usuarioId, string nuevoEstado);
        Task<Dispositivo> CrearDesdeProductoUnidadAsync(int productoUnidadId, string usuarioId);
    }
}

