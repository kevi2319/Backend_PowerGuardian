using Microsoft.AspNetCore.Mvc;
using Backend_PowerGuardian.Services;

namespace Backend_PowerGuardian.Controllers
{
    [ApiController]
    [Route("api/control")]
    public class RelayController : ControllerBase
    {
        private readonly MqttService _mqtt;

        public RelayController(MqttService mqtt)
        {
            _mqtt = mqtt;
        }

        [HttpPost("relay")]
        public async Task<IActionResult> ControlRelay([FromBody] RelayRequest request)
        {
            if (request == null || (request.Estado != "ON" && request.Estado != "OFF"))
                return BadRequest("Estado inválido");

            await _mqtt.PublicarRelay(request.Estado);
            return Ok(new { mensaje = $"Relé cambiado a {request.Estado}" });
        }
    }

    public class RelayRequest
    {
        public string Estado { get; set; }  // "ON" o "OFF"
    }
}
