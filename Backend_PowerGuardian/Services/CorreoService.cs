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
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                        <h2 style='color: #333;'>Gracias por tu compra de PowerGuardian 🛒</h2>
                        <p style='font-size: 16px; color: #555;'>Para activar tu dispositivo, crea tu cuenta en:</p>
                        <p>
                            <a href='https://powerguardian.app/registro' 
                            style='font-size: 16px; color: #007BFF; text-decoration: none;'>
                                https://powerguardian.app/registro
                            </a>
                        </p>
                        <p style='font-size: 16px; color: #555;'>Usa este código SKU durante el registro:</p>
                        <div style='background-color: #e0f7fa; padding: 10px; border-radius: 5px; margin-top: 10px;'>
                            <h1 style='text-align: center; color: #00796b;'>{sku}</h1>
                        </div>
                    </div>
                </body>
                </html>";

            await EnviarCorreo(email, asunto, cuerpo);
        }


        public async Task EnviarCorreoAsociarDispositivo(string email, string sku)
        {
            var asunto = "Asocia tu nuevo PowerGuardian";
            var cuerpo = $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                        <h2 style='color: #333;'>¡Gracias por tu nueva compra!</h2>
                        <p style='font-size: 16px; color: #555;'>Inicia sesión en tu cuenta desde el siguiente enlace:</p>
                        <p>
                            <a href='https://powerguardian.app/login' 
                            style='font-size: 16px; color: #007BFF; text-decoration: none;'>
                            https://powerguardian.app/login
                            </a>
                        </p>
                        <p style='font-size: 16px; color: #555;'>Luego, ve a la sección <strong>“Agregar dispositivo”</strong> e ingresa este SKU:</p>
                        <div style='background-color: #e8f5e9; padding: 12px; border-radius: 6px; margin-top: 10px;'>
                            <h1 style='text-align: center; color: #2e7d32;'>{sku}</h1>
                        </div>
                    </div>
                </body>
                </html>";

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
