using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text;

namespace Backend_PowerGuardian.Services
{
    public class MqttService
    {
        private readonly IMqttClient _client;
        private readonly string _topicStatus = "powerguardian/status";
        private readonly string _topicControl = "powerguardian/relay";

        public MqttService()
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            _client.ApplicationMessageReceivedAsync += e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var topic = e.ApplicationMessage.Topic;

                Console.WriteLine($"📥 [{topic}] = {payload}");

                // Aquí podrías guardar el estado en base de datos

                return Task.CompletedTask;
            };

            _client.ConnectedAsync += async e =>
            {
                Console.WriteLine("✅ Conectado al broker MQTT");
                await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => f
                        .WithTopic(_topicStatus)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce))
                    .Build());
            };
        }

        public async Task InitAsync()
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("934c2afea6844e3ab019b059296c5461.s1.eu.hivemq.cloud", 8883)
                .WithCredentials("PowerGuardian", "PowerGuardian01") // Usa tus credenciales MQTT
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    AllowUntrustedCertificates = true,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true
                })
                .Build();

            await _client.ConnectAsync(options, CancellationToken.None);
        }

        public async Task PublicarRelay(string mensaje)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_topicControl)
                .WithPayload(mensaje)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(true)
                .Build();

            if (_client.IsConnected)
            {
                await _client.PublishAsync(message, CancellationToken.None);
                Console.WriteLine($"📤 Publicado a [{_topicControl}]: {mensaje}");
            }
            else
            {
                Console.WriteLine("⚠️ Cliente MQTT no conectado");
            }
        }
    }
}
