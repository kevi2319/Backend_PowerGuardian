using Microsoft.AspNetCore.Mvc;
using Backend_PowerGuardian.Services;

namespace Backend_PowerGuardian.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CosteoController : ControllerBase
    {
        private readonly ICosteoService _costeoService;

        public CosteoController(ICosteoService costeoService)
        {
            _costeoService = costeoService;
        }

        [HttpGet("{productoId}")]
        public async Task<IActionResult> ObtenerCostoProducto(int productoId)
        {
            try
            {
                var costo = await _costeoService.CalcularCostoProductoAsync(productoId);
                return Ok(new { productoId, costo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}