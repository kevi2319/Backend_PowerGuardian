using System.Net;
using System.Net.Mail;
using Backend_PowerGuardian.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Backend_PowerGuardian.Services
{
    public class CosteoService : ICosteoService
    {
        private readonly ApplicationDbContext _context;

        public CosteoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> CalcularCostoProductoAsync(int productoId)
        {
            var receta = await _context.RecetasProducto
                .Where(r => r.ProductoId == productoId)
                .Include(r => r.MateriaPrima)
                .ToListAsync();

            if (receta == null || receta.Count == 0)
                throw new Exception("El producto no tiene una receta registrada.");

            decimal total = 0;
            foreach (var item in receta)
            {
                total += item.Cantidad * item.MateriaPrima.CostoUnitario;
            }

            return total;
        }
    }

}
