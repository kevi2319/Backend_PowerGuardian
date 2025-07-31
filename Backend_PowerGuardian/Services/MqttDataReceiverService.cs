using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Backend_PowerGuardian.Services
{
    public class MqttDataReceiverService : BackgroundService
    {
        private readonly ILogger<MqttDataReceiverService> _logger;
        private IMqttClient _mqttClient;
        private MqttClientOptions _mqttOptions;
        private readonly IServiceProvider _serviceProvider; // Para obtener el DbContext
        
        public MqttDataReceiverService(ILogger<MqttDataReceiverService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio MQTT iniciado.");

            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            //CONFIGURACION DEL BROKER MQTT (version v3.1.5)
            _mqttOptions = mqttFactory.CreateClientOptionsBuilder()
                .WithClientId("BackendService_" + Guid.NewGuid()) // ID unico para el backend
                .WithTcpServer("934c2afea6844e3ab019b059296c5461.s1.eu.hivemq.cloud", 8883) //
                .WithCredentials("PowerGuardian", "PowerGuardian01")
                .WithCleanSession()
                .WithTlsOptions( options =>
                {
                    options.WithCertificateValidationHandler(c => true) // Solo para desarrollo. En producción, valida el certificado.
                           .WithAllowUntrustedCertificates() // Solo para desarrollo.
                           .WithIgnoreCertificateChainErrors() // Solo para desarrollo.
                           .WithIgnoreCertificateRevocationErrors(); // Solo para desarrollo.
                })
                .Build();

            //REGISTRAR EL EVENTO
            _mqttClient.ConnectedAsync += async args =>
            {
                _logger.LogInformation("Conectando a MQTT Broker.");

                var topicFilter = new MqttTopicFilterBuilder()
                    .WithTopic("PowerGuardian/+/data")
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                    .Build();
                  
                await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(topicFilter)
                    .Build());

                _logger.LogInformation("Suscrito al topic PowerGuardian/+/data");
            };

            _mqttClient.DisconnectedAsync += async args =>
            {
                _logger.LogWarning("Desconectado de MQTT Broker. Reintentando conexión en 5 segundos...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                try
                {
                    await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fallo al reconectar a MQTT Broker.");
                }
            };

            _mqttClient.ApplicationMessageReceivedAsync += async args =>
            {
                string topic = args.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
               
                _logger.LogInformation($"Mensaje recibido - Topic: {topic}, Payload: {payload}");

                try
                {
                    // Deserializar el JSON a tu objeto PzemData
                    var pzemData = JsonConvert.DeserializeObject<PzemData>(payload);

                    // --- GUARDAR EN LA BASE DE DATOS ---
                    // Necesitarás un DbContext para interactuar con SQL Server
                    // Usa un scope para obtener el DbContext, ya que BackgroundService es Singleton
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Reemplaza ApplicationDbContext con el nombre de tu DbContext

                    // LOGICA DE VINCULACION DEL DEVICEID (MAC) AL SKU
                    // 1. PRODUCTO YA VINCULADO A ESTE DEVICEID
                    ProductoUnidad? productoUnidad = null;                 
                    productoUnidad = await dbContext.ProductoUnidades.FirstOrDefaultAsync(u => u.DeviceId == pzemData.DeviceId);

                    if (productoUnidad != null)
                    {
                        // 2. EN CASO DE QUE NO SE ENCUENTRE UN DEVICEID, ASIGNE UN ESENARIO DONDE UN USUARIO REGISTRARA UN SKU Y LUEGO SE CONECTARA CON EL ESP32
                        _logger.LogInformation($"DeviceId '{pzemData.DeviceId}' ya vinculado a SKU '{productoUnidad.SKU}'.");

                    } 
                    else
                    {
                        productoUnidad = await dbContext.ProductoUnidades.FirstOrDefaultAsync(u => u.Usado && u.DeviceId == null && u.UserId != null);

                        if (productoUnidad != null)
                        {
                            // SI SE ENCUENTRA UNA UNIDAD USASDA EL DEVICE LA VINCULARA
                            productoUnidad.DeviceId = pzemData.DeviceId;
                            await dbContext.SaveChangesAsync();
                            _logger.LogInformation($"SKU '{productoUnidad.SKU}' vinculando a DeviceId '{pzemData.DeviceId}' para el usuario '{productoUnidad.UserId}'.");
                        }
                        else
                        {
                            _logger.LogWarning($"DeviceId '{pzemData.DeviceId}' recibido, pero no se encontro un SKU sin vincular o ya estaba vinculado a otro. La lectura se guardara sin una vinculacion directa a ProsuctoUnidad.");
                        }
                    }

                    // Aquí mapeas los datos recibidos a tu entidad de base de datos
                    // Asegúrate de tener una entidad para PzemReadings en tu DbContext
                    var newReading = new PzemReading // Asume que tienes una clase PzemReading en tu modelo de BD
                    {
                        DeviceId = pzemData.DeviceId,
                        Voltaje = pzemData.Voltaje,
                        Corriente = pzemData.Corriente,
                        Potencia = pzemData.Potencia,
                        FactorPotencia = pzemData.FactorPotencia,
                        Frecuencia = pzemData.Frecuencia,
                        Energia = pzemData.Energia,
                        Timestamp = DateTimeOffset.UtcNow // Usa el tiempo de recepción del servidor
                                                              // O si quieres el timestamp del ESP32: Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(pzemData.Timestamp))
                    };
                    dbContext.PzemReadings.Add(newReading);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Datos de {pzemData.DeviceId} guardados en la BD.");
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error al deserializar el mensaje JSON MQTT.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar el mensaje MQTT o guardar en la BD.");
                }
            };

            //INTENTAR CONECTAR
            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar al broker MQTT.");
            }
            // Esperar indefinidamente mientras el servicio está en ejecución
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Servicio MQTT deteniendose.");
            if (_mqttClient?.IsConnected == true)
            {
                await _mqttClient.DisconnectAsync();
            }
        }
    }
}
