using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace Backend_PowerGuardian.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceControlController : ControllerBase
    {
        private readonly ILogger<DeviceControlController> _logger;
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
    

        // CONSTRUCTOR PARA LA INYECCION DEL LOGGER Y EL CLIENTE MQTT
        public DeviceControlController(ILogger<DeviceControlController> logger, IMqttClient mqttClient)
        {
            _logger = logger;
            _mqttClient = mqttClient;

            //CONFIGURACION PARA LA CONEXION DEL CLIENTE MQTT
            var mqttFactory = new MqttFactory();
            _mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId("BackendPublisher_" + Guid.NewGuid().ToString())
                .WithTcpServer("934c2afea6844e3ab019b059296c5461.s1.eu.hivemq.cloud", 8883) // URL y PUERTO TLS de HiveMQ Cloud!
                .WithCredentials("TU_USUARIO_DE_HIVE_MQ", "TU_CONTRASEÑA_DE_HIVE_MQ") // CREDENCIALES REALES 
                .WithCleanSession()
                .WithTlsOptions(options => // CONFIGURACION TLS PARA LA CONEXION SEGURA
                {
                    options.WithCertificateValidationHandler(c => true) // Solo para desarrollo. En producción, valida el certificado.
                           .WithAllowUntrustedCertificates() // Solo para desarrollo.
                           .WithIgnoreCertificateChainErrors() // Solo para desarrollo.
                           .WithIgnoreCertificateRevocationErrors(); // Solo para desarrollo.
                })
                .Build();
        }

        [HttpPost("relay/{deviceId}/{command}")]
        public async Task<IActionResult> ControlRelay(string deviceId, string command)
        {
            // VALIDACION DE LOS PARAMETROS
            if (string.IsNullOrWhiteSpace(deviceId)) {
                return BadRequest("El ID del dispositivo no puede estar vacío.");
            }

            if (command != "ON" && command != "OFF")
            {
                return BadRequest("Comando invalido. Usa 'ON' o 'OFF'.");
            }

            // A SEGURARSE QUE EL CLIENTE MQTT CONECTADO ANTES DE SER PUBLICADO
            if (!_mqttClient.IsConnected) 
            {
                _logger.LogInformation("Cliente MQTT publicador no conectado. Intentando reconecar...");
                try
                {
                    await _mqttClient.ConnectAsync(_mqttOptions, HttpContext.RequestAborted);
                    if (!_mqttClient.IsConnected)
                    {
                        _logger.LogError("Fallo al reconectar el cliente MQTT publicador.");
                        return StatusCode(500, "No se pudo conectar al broker MQTT para enviar el comando.");

                    }
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Excepción al intentar conectar el cliente MQTT publicador.");
                    return StatusCode(500, "Error interno al conectar con el broker MQTT para enviar el comando.");
                }
            }

            // CONTRUCCION DEL TOPICO DE CONTROL PARA EL DISPOSITIVO ESPECIFICO
            string topic = $"PowerGuardian/{deviceId}/control";

            // MENSAJE MQTT
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(command)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false)
                .Build();

            try
            {
                // PUBLICA EL MENSAJE MQTT
                await _mqttClient.PublishAsync(message, HttpContext.RequestAborted);
                _logger.LogInformation($"Comando '{command}' publicado a tópico '{topic}' para deviceId: '{deviceId}'.");
                return Ok($"Comando '{command}' enviado al dispositivo '{deviceId}' correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al publicar comando '{command}' para deviceId: '{deviceId}'.");
                return StatusCode(500, "Error al enviar el comando MQTT.");
            }
        }  
    }
}
