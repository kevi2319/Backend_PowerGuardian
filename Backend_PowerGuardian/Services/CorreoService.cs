using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Backend_PowerGuardian.Services
{
    public class CorreoService : ICorreoService
    {
        private readonly IConfiguration _config;

        public CorreoService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoRegistroConSku(string email, string sku)
        {
            var asunto = "Activa tu PowerGuardian";
            var cuerpo = $@"
                <p>Gracias por tu compra de PowerGuardian 🛒</p>
                <p>Para activar tu dispositivo, crea tu cuenta en:<br>
                <a href='https://powerguardian.app/registro'>https://powerguardian.app/registro</a></p>
                <p>Usa este código SKU durante el registro:</p>
                <h2>{sku}</h2>";

            await EnviarCorreo(email, asunto, cuerpo);
        }

        public async Task EnviarCorreoAsociarDispositivo(string email, string sku)
        {
            var asunto = "Asocia tu nuevo PowerGuardian";
            var cuerpo = $@"
                <p>¡Gracias por tu nueva compra!</p>
                <p>Inicia sesión en:<br>
                <a href='https://powerguardian.app/login'>https://powerguardian.app/login</a></p>
                <p>Ve a la sección ‘Agregar dispositivo’ e ingresa este SKU:</p>
                <h2>{sku}</h2>";

            await EnviarCorreo(email, asunto, cuerpo);
        }

        private async Task EnviarCorreo(string destino, string asunto, string cuerpoHtml)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"]);
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Pass"];
            var from = _config["Smtp:From"];

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var message = new MailMessage(from, destino)
            {
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}
