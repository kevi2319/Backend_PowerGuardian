using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.Services;
using Backend_PowerGuardian.DTOs;
using System.Globalization;

namespace Backend_PowerGuardian.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactoController : ControllerBase
    {
        private readonly ICorreoService _correoService;
        private readonly ICosteoService _costeoService;

        public ContactoController(ICorreoService correoService, ICosteoService costeoService)
        {
            _correoService = correoService;
            _costeoService = costeoService;
        }

        // Método para formatear cantidades en pesos mexicanos
        private string FormatearPesos(decimal cantidad)
        {
            return cantidad.ToString("C", new CultureInfo("es-MX"));
        }

        [HttpPost]
        public async Task<IActionResult> Enviar([FromBody] ContactoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Asunto))
                return BadRequest(new { mensaje = "Debes seleccionar un asunto" });

            if (dto.Asunto == "soporte")
            {
                // Correo interno para el equipo de soporte
                var cuerpoInterno = $@"
                    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 24px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: 600;'>⚡ PowerGuardian</h1>
                            <p style='color: #e8f5e8; margin: 8px 0 0 0; font-size: 14px;'>Nueva Solicitud de Soporte</p>
                        </div>
                        
                        <!-- Content -->
                        <div style='padding: 32px 24px;'>
                            <div style='background: #f8fffe; border-left: 4px solid #67b061; padding: 20px; border-radius: 8px; margin-bottom: 24px;'>
                                <h3 style='color: #67b061; margin: 0 0 16px 0; font-size: 18px;'>📞 Detalles del Cliente</h3>
                                <p style='margin: 8px 0; color: #333;'><strong>Nombre:</strong> {dto.Nombre}</p>
                                <p style='margin: 8px 0; color: #333;'><strong>Correo:</strong> <a href='mailto:{dto.Correo}' style='color: #3d7bb6; text-decoration: none;'>{dto.Correo}</a></p>
                            </div>
                            
                            <div style='background: #fff8f0; border-left: 4px solid #3d7bb6; padding: 20px; border-radius: 8px;'>
                                <h3 style='color: #3d7bb6; margin: 0 0 16px 0; font-size: 18px;'>💬 Mensaje</h3>
                                <div style='background: #ffffff; padding: 16px; border-radius: 6px; border: 1px solid #e1e5e9; color: #333; line-height: 1.6;'>
                                    {dto.Mensaje}
                                </div>
                            </div>
                        </div>
                        
                        <!-- Footer -->
                        <div style='background: #f8f9fa; padding: 16px 24px; text-align: center; border-top: 1px solid #e1e5e9;'>
                            <p style='margin: 0; color: #6c757d; font-size: 12px;'>Sistema de Soporte PowerGuardian | Responder con prioridad</p>
                        </div>
                    </div>
                ";
                await _correoService.EnviarCorreo("soporte@powerguardian.mx", "🆘 Nueva solicitud de soporte - " + dto.Nombre, cuerpoInterno);

                // Correo de confirmación al cliente
                var cuerpoCliente = $@"
                    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 32px 24px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;'>⚡ PowerGuardian</h1>
                            <p style='color: #e8f5e8; margin: 12px 0 0 0; font-size: 16px;'>Tu solicitud ha sido recibida</p>
                        </div>
                        
                        <!-- Content -->
                        <div style='padding: 40px 24px;'>
                            <div style='text-align: center; margin-bottom: 32px;'>
                                <div style='display: inline-block; background: #e8f5e8; color: #67b061; padding: 12px 20px; border-radius: 25px; font-weight: 600; font-size: 14px;'>
                                    ✅ SOLICITUD CONFIRMADA
                                </div>
                            </div>
                            
                            <h2 style='color: #333; margin: 0 0 16px 0; font-size: 20px;'>Hola <span style='color: #67b061;'>{dto.Nombre}</span>,</h2>
                            
                            <p style='color: #555; line-height: 1.6; margin: 16px 0;'>
                                Gracias por contactar a <strong>PowerGuardian</strong>. Hemos recibido tu solicitud de soporte y nuestro equipo especializado se pondrá en contacto contigo en las próximas <strong>24 horas</strong>.
                            </p>
                            
                            <div style='background: #f8fffe; border: 1px solid #c7feb6; border-radius: 8px; padding: 20px; margin: 24px 0;'>
                                <h3 style='color: #67b061; margin: 0 0 12px 0; font-size: 16px;'>📋 Resumen de tu solicitud:</h3>
                                <div style='background: #ffffff; padding: 16px; border-radius: 6px; border-left: 4px solid #67b061; color: #333; line-height: 1.6; font-style: italic;'>
                                    ""{dto.Mensaje}""
                                </div>
                            </div>
                            
                            <div style='background: #fff8f0; border: 1px solid #ffd700; border-radius: 8px; padding: 20px; margin: 24px 0;'>
                                <h3 style='color: #3d7bb6; margin: 0 0 12px 0; font-size: 16px;'>⏱️ ¿Qué sigue?</h3>
                                <ul style='color: #555; line-height: 1.6; margin: 0; padding-left: 20px;'>
                                    <li>Nuestro equipo técnico revisará tu solicitud</li>
                                    <li>Te contactaremos por correo o teléfono</li>
                                    <li>Resolveremos tu consulta de manera rápida y efectiva</li>
                                </ul>
                            </div>
                            
                            <p style='color: #555; line-height: 1.6; margin: 24px 0 0 0;'>
                                Si tienes alguna pregunta urgente, no dudes en responder a este correo.
                            </p>
                        </div>
                        
                        <!-- Footer -->
                        <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 24px; text-align: center;'>
                            <p style='color: #ffffff; margin: 0 0 8px 0; font-weight: 600;'>Saludos cordiales,</p>
                            <p style='color: #e8f5e8; margin: 0; font-size: 14px;'>Equipo de Soporte PowerGuardian</p>
                            <div style='margin-top: 16px; padding-top: 16px; border-top: 1px solid rgba(255,255,255,0.2);'>
                                <p style='color: #e8f5e8; margin: 0; font-size: 12px;'>🔋 Energía inteligente, control total</p>
                            </div>
                        </div>
                    </div>
                ";
                await _correoService.EnviarCorreo(dto.Correo, "✅ Confirmación de solicitud - PowerGuardian", cuerpoCliente);

                return Ok(new { mensaje = "¡Gracias por contactarnos! 🚀 Revisaremos tu solicitud y nos pondremos en contacto contigo en las próximas 24 horas." });
            }

            // Método para formatear cantidades en pesos mexicanos
            if (dto.Asunto == "cotizacion")
            {
                if (dto.ProductoId == null || dto.Cantidad == null || dto.Cantidad <= 0)
                    return BadRequest(new { mensaje = "Faltan datos para generar la cotización." });

                var costoUnitario = await _costeoService.CalcularCostoProductoAsync(dto.ProductoId.Value);
                var total = costoUnitario * dto.Cantidad.Value;

                // Formatear cantidades con el formato mexicano
                var costoFormateado = FormatearPesos(costoUnitario);
                var totalFormateado = FormatearPesos(total);

                var cuerpo = $@"
                    <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 32px 24px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;'>⚡ PowerGuardian</h1>
                            <p style='color: #e8f5e8; margin: 12px 0 0 0; font-size: 16px;'>Tu Cotización Personalizada</p>
                        </div>
                        
                        <!-- Content -->
                        <div style='padding: 40px 24px;'>
                            <div style='text-align: center; margin-bottom: 32px;'>
                                <div style='display: inline-block; background: #e8f5e8; color: #67b061; padding: 12px 20px; border-radius: 25px; font-weight: 600; font-size: 14px;'>
                                    💰 COTIZACIÓN LISTA
                                </div>
                            </div>
                            
                            <h2 style='color: #333; margin: 0 0 16px 0; font-size: 20px;'>Hola <span style='color: #67b061;'>{dto.Nombre}</span>,</h2>
                            
                            <p style='color: #555; line-height: 1.6; margin: 16px 0;'>
                                Gracias por tu interés en <strong>PowerGuardian</strong>. Hemos preparado una cotización personalizada para ti:
                            </p>
                            
                            <!-- Detalles del producto -->
                            <div style='background: #f8fffe; border: 1px solid #c7feb6; border-radius: 12px; padding: 24px; margin: 24px 0;'>
                                <h3 style='color: #67b061; margin: 0 0 20px 0; font-size: 18px; text-align: center;'>📦 Detalles del Producto</h3>
                                
                                <div style='background: #ffffff; border-radius: 8px; padding: 20px; border: 1px solid #e1e5e9;'>
                                    <div style='display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding: 12px 0; border-bottom: 1px solid #f1f3f4;'>
                                        <span style='color: #666; font-weight: 500;'>Producto ID:</span>
                                        <span style='color: #333; font-weight: 600;'>{dto.ProductoId}</span>
                                    </div>
                                    
                                    <div style='display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding: 12px 0; border-bottom: 1px solid #f1f3f4;'>
                                        <span style='color: #666; font-weight: 500;'>Cantidad:</span>
                                        <span style='color: #3d7bb6; font-weight: 600; font-size: 16px;'>{dto.Cantidad} unidades</span>
                                    </div>
                                    
                                    <div style='display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding: 12px 0; border-bottom: 1px solid #f1f3f4;'>
                                        <span style='color: #666; font-weight: 500;'>Precio unitario:</span>
                                        <span style='color: #67b061; font-weight: 600; font-size: 16px;'>{costoFormateado}</span>
                                    </div>
                                    
                                    <div style='display: flex; justify-content: space-between; align-items: center; background: #e8f5e8; padding: 16px; border-radius: 8px; margin-top: 16px;'>
                                        <span style='color: #333; font-weight: 600; font-size: 18px;'>💰 Total:</span>
                                        <span style='color: #67b061; font-weight: 700; font-size: 24px;'>{totalFormateado}</span>
                                    </div>
                                </div>
                            </div>
                            
                            <!-- Información adicional -->
                            <div style='background: #fff8f0; border: 1px solid #ffd700; border-radius: 8px; padding: 20px; margin: 24px 0;'>
                                <h3 style='color: #3d7bb6; margin: 0 0 12px 0; font-size: 16px;'>📞 Próximos pasos:</h3>
                                <ul style='color: #555; line-height: 1.6; margin: 0; padding-left: 20px;'>
                                    <li>Nuestro equipo comercial te contactará en las próximas <strong>24 horas</strong></li>
                                    <li>Revisaremos juntos los detalles técnicos</li>
                                    <li>Coordinaremos la entrega y instalación</li>
                                    <li>Te brindaremos soporte completo post-venta</li>
                                </ul>
                            </div>
                            
                            <div style='background: #e8f5e8; border-radius: 8px; padding: 20px; margin: 24px 0; text-align: center;'>
                                <p style='color: #67b061; margin: 0; font-weight: 600; font-size: 14px;'>
                                    🎯 Esta cotización es válida por 30 días
                                </p>
                            </div>
                            
                            <p style='color: #555; line-height: 1.6; margin: 24px 0 0 0; text-align: center;'>
                                ¿Tienes alguna pregunta? No dudes en contactarnos.
                            </p>
                        </div>
                        
                        <!-- Footer -->
                        <div style='background: linear-gradient(135deg, #67b061 0%, #3d7bb6 100%); padding: 24px; text-align: center;'>
                            <p style='color: #ffffff; margin: 0 0 8px 0; font-weight: 600;'>Gracias por confiar en PowerGuardian</p>
                            <p style='color: #e8f5e8; margin: 0; font-size: 14px;'>Equipo Comercial PowerGuardian</p>
                            <div style='margin-top: 16px; padding-top: 16px; border-top: 1px solid rgba(255,255,255,0.2);'>
                                <p style='color: #e8f5e8; margin: 0; font-size: 12px;'>🔋 Energía inteligente, control total</p>
                            </div>
                        </div>
                    </div>
                ";

                await _correoService.EnviarCorreo(dto.Correo, "💰 Tu Cotización PowerGuardian - " + totalFormateado, cuerpo);
                return Ok(new { mensaje = "¡Cotización enviada! Revisa tu correo " + dto.Correo + " y nos pondremos en contacto contigo pronto. 🚀" });
            }

            return BadRequest(new { mensaje = "Asunto no válido." });
        }
    }

}
