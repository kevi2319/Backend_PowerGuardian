using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Backend_PowerGuardian.Services
{
    public class EmailService : ICorreoService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoRegistroConSku(string email, string sku)
        {
            var asunto = "Activa tu PowerGuardian";
            var cuerpo = $@"
                <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 32px 24px; text-align: center;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;'>⚡ PowerGuardian</h1>
                        <p style='color: #e8f5e8; margin: 12px 0 0 0; font-size: 16px;'>¡Activa tu dispositivo!</p>
                    </div>
                    
                    <!-- Content -->
                    <div style='padding: 40px 24px;'>
                        <div style='text-align: center; margin-bottom: 32px;'>
                            <div style='display: inline-block; background: #e8f5e8; color: #67b061; padding: 12px 20px; border-radius: 25px; font-weight: 600; font-size: 14px;'>
                                🛒 COMPRA CONFIRMADA
                            </div>
                        </div>
                        
                        <h2 style='color: #333; margin: 0 0 16px 0; font-size: 20px; text-align: center;'>¡Gracias por elegir <span style='color: #67b061;'>PowerGuardian</span>!</h2>
                        
                        <p style='color: #555; line-height: 1.6; margin: 16px 0; text-align: center;'>
                            Para activar tu dispositivo y comenzar a monitorear tu energía, necesitas crear tu cuenta:
                        </p>
                        
                        <div style='background: #f8fffe; border: 1px solid #c7feb6; border-radius: 12px; padding: 24px; margin: 24px 0; text-align: center;'>
                            <h3 style='color: #67b061; margin: 0 0 16px 0; font-size: 18px;'>🔗 Paso 1: Regístrate</h3>
                            <a href='http://localhost:4200/login/register' style='display: inline-block; background: #67b061; color: #ffffff; padding: 14px 28px; border-radius: 8px; text-decoration: none; font-weight: 600; margin: 8px 0; transition: background-color 0.3s;'>
                                Crear mi cuenta PowerGuardian
                            </a>
                        </div>
                        
                        <div style='background: #fff8f0; border: 1px solid #ffd700; border-radius: 12px; padding: 24px; margin: 24px 0; text-align: center;'>
                            <h3 style='color: #3d7bb6; margin: 0 0 16px 0; font-size: 18px;'>🔑 Paso 2: Usa tu código SKU</h3>
                            <p style='color: #555; margin: 8px 0 16px 0;'>Ingresa este código durante el registro:</p>
                            <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); color: #ffffff; padding: 20px; border-radius: 8px; margin: 16px 0;'>
                                <h1 style='text-align: center; margin: 0; font-size: 32px; font-weight: 700; letter-spacing: 2px;'>{sku}</h1>
                            </div>
                            <p style='color: #666; font-size: 12px; margin: 8px 0 0 0;'>💡 Guarda este código, lo necesitarás para activar tu dispositivo</p>
                        </div>
                        
                        <div style='background: #e8f5e8; border-radius: 8px; padding: 20px; margin: 24px 0; text-align: center;'>
                            <h3 style='color: #67b061; margin: 0 0 12px 0; font-size: 16px;'>🚀 ¿Qué obtienes con PowerGuardian?</h3>
                            <ul style='text-align: left; color: #555; line-height: 1.6; margin: 12px 0; padding-left: 20px; max-width: 400px; margin-left: auto; margin-right: auto;'>
                                <li>Monitoreo en tiempo real de tu consumo eléctrico</li>
                                <li>Alertas inteligentes de uso excesivo</li>
                                <li>Reportes detallados para optimizar tu energía</li>
                                <li>Control remoto desde cualquier lugar</li>
                            </ul>
                        </div>
                    </div>
                    
                    <!-- Footer -->
                    <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 24px; text-align: center;'>
                        <p style='color: #ffffff; margin: 0 0 8px 0; font-weight: 600;'>¡Bienvenido a la familia PowerGuardian!</p>
                        <p style='color: #e8f5e8; margin: 0; font-size: 14px;'>Equipo PowerGuardian</p>
                        <div style='margin-top: 16px; padding-top: 16px; border-top: 1px solid rgba(255,255,255,0.2);'>
                            <p style='color: #e8f5e8; margin: 0; font-size: 12px;'>🔋 Energía inteligente, control total</p>
                        </div>
                    </div>
                </div>
            ";

            await EnviarCorreo(email, asunto, cuerpo);
        }

        public async Task EnviarCorreoAsociarDispositivo(string email, string sku)
        {
            var asunto = "Asocia tu nuevo PowerGuardian";
            var cuerpo = $@"
                <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 32px 24px; text-align: center;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;'>⚡ PowerGuardian</h1>
                        <p style='color: #e8f5e8; margin: 12px 0 0 0; font-size: 16px;'>¡Nuevo dispositivo disponible!</p>
                    </div>
                    
                    <!-- Content -->
                    <div style='padding: 40px 24px;'>
                        <div style='text-align: center; margin-bottom: 32px;'>
                            <div style='display: inline-block; background: #e8f5e8; color: #67b061; padding: 12px 20px; border-radius: 25px; font-weight: 600; font-size: 14px;'>
                                🆕 DISPOSITIVO LISTO
                            </div>
                        </div>
                        
                        <h2 style='color: #333; margin: 0 0 16px 0; font-size: 20px; text-align: center;'>¡Gracias por tu <span style='color: #67b061;'>nueva compra</span>!</h2>
                        
                        <p style='color: #555; line-height: 1.6; margin: 16px 0; text-align: center;'>
                            Tu nuevo dispositivo PowerGuardian está listo para ser asociado a tu cuenta existente.
                        </p>
                        
                        <div style='background: #f8fffe; border: 1px solid #c7feb6; border-radius: 12px; padding: 24px; margin: 24px 0; text-align: center;'>
                            <h3 style='color: #67b061; margin: 0 0 16px 0; font-size: 18px;'>🔐 Paso 1: Inicia sesión</h3>
                            <a href='http://localhost:4200/login' style='display: inline-block; background: #3d7bb6; color: #ffffff; padding: 14px 28px; border-radius: 8px; text-decoration: none; font-weight: 600; margin: 8px 0; transition: background-color 0.3s;'>
                                Acceder a mi cuenta
                            </a>
                        </div>
                        
                        <div style='background: #fff8f0; border: 1px solid #ffd700; border-radius: 12px; padding: 24px; margin: 24px 0; text-align: center;'>
                            <h3 style='color: #3d7bb6; margin: 0 0 16px 0; font-size: 18px;'>➕ Paso 2: Agregar dispositivo</h3>
                            <p style='color: #555; margin: 8px 0 16px 0;'>Ve a la sección <strong>&quot;Agregar dispositivo&quot;</strong> e ingresa este SKU:</p>
                            <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); color: #ffffff; padding: 20px; border-radius: 8px; margin: 16px 0;'>
                                <h1 style='text-align: center; margin: 0; font-size: 32px; font-weight: 700; letter-spacing: 2px;'>{sku}</h1>
                            </div>
                            <p style='color: #666; font-size: 12px; margin: 8px 0 0 0;'>💡 Este código vinculará el nuevo dispositivo a tu cuenta</p>
                        </div>
                        
                        <div style='background: #e8f5e8; border-radius: 8px; padding: 20px; margin: 24px 0; text-align: center;'>
                            <h3 style='color: #67b061; margin: 0 0 12px 0; font-size: 16px;'>📱 Gestiona todos tus dispositivos</h3>
                            <p style='color: #555; line-height: 1.6; margin: 12px 0;'>
                                Desde tu panel de control podrás monitorear todos tus dispositivos PowerGuardian, 
                                comparar consumos y optimizar tu energía de manera inteligente.
                            </p>
                        </div>
                        
                        <p style='color: #555; line-height: 1.6; margin: 24px 0 0 0; text-align: center;'>
                            ¿Necesitas ayuda? Contáctanos y te asistiremos en el proceso.
                        </p>
                    </div>
                    
                    <!-- Footer -->
                    <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 24px; text-align: center;'>
                        <p style='color: #ffffff; margin: 0 0 8px 0; font-weight: 600;'>¡Sigue creciendo tu red PowerGuardian!</p>
                        <p style='color: #e8f5e8; margin: 0; font-size: 14px;'>Equipo PowerGuardian</p>
                        <div style='margin-top: 16px; padding-top: 16px; border-top: 1px solid rgba(255,255,255,0.2);'>
                            <p style='color: #e8f5e8; margin: 0; font-size: 12px;'>🔋 Energía inteligente, control total</p>
                        </div>
                    </div>
                </div>
            ";

            await EnviarCorreo(email, asunto, cuerpo);
        }

        public async Task EnviarCorreo(string destinatario, string asunto, string cuerpoHtml)
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

            var message = new MailMessage(from, destinatario)
            {
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }

        private string FormatearPesos(decimal cantidad)
        {
            var cultureInfo = new CultureInfo("es-MX");
            return cantidad.ToString("C", cultureInfo);
        }
    }
}
