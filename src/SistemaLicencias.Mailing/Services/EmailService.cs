using EmailAPI.Configuration;
using EmailAPI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailAPI.Services
{
    public interface IEmailService
    {
        Task<EmailResponse> EnviarEmailAsync(EmailRequest request);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly string _rutaPlantillas;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger,
            IWebHostEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _rutaPlantillas = Path.Combine(env.ContentRootPath, "Resources", "Templates");
        }

        public async Task<EmailResponse> EnviarEmailAsync(EmailRequest request)
        {
            try
            {
                _logger.LogInformation($"Iniciando envío de email a {request.Para}");

                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(
                    _emailSettings.NombreRemitente,
                    _emailSettings.EmailRemitente));
                mensaje.To.Add(MailboxAddress.Parse(request.Para));
                mensaje.Subject = request.Asunto;

                // Agregar CC si existen
                if (request.Cc != null && request.Cc.Any())
                {
                    foreach (var cc in request.Cc)
                    {
                        mensaje.Cc.Add(MailboxAddress.Parse(cc));
                    }
                }

                // Agregar BCC si existen
                if (request.Bcc != null && request.Bcc.Any())
                {
                    foreach (var bcc in request.Bcc)
                    {
                        mensaje.Bcc.Add(MailboxAddress.Parse(bcc));
                    }
                }

                // Crear el cuerpo del mensaje
                var bodyBuilder = new BodyBuilder();

                // Si se especifica una plantilla, cargarla y procesarla
                if (!string.IsNullOrEmpty(request.NombrePlantilla))
                {
                    var contenidoHtml = await CargarPlantillaAsync(request.NombrePlantilla);
                    contenidoHtml = ProcesarPlantilla(contenidoHtml, request);
                    bodyBuilder.HtmlBody = contenidoHtml;
                }
                else
                {
                    // Si no hay plantilla, usar el contenido directamente
                    // Verificar si el contenido parece HTML
                    if (request.Contenido.Trim().StartsWith("<"))
                    {
                        bodyBuilder.HtmlBody = request.Contenido;
                    }
                    else
                    {
                        bodyBuilder.TextBody = request.Contenido;
                    }
                }

                mensaje.Body = bodyBuilder.ToMessageBody();

                // Enviar el email
                using var cliente = new SmtpClient();
                
                await cliente.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.SmtpPort,
                    _emailSettings.UsarSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                await cliente.AuthenticateAsync(
                    _emailSettings.Usuario,
                    _emailSettings.Contraseña);

                await cliente.SendAsync(mensaje);
                await cliente.DisconnectAsync(true);

                _logger.LogInformation($"Email enviado exitosamente a {request.Para}");

                return new EmailResponse
                {
                    Exitoso = true,
                    Mensaje = "Email enviado correctamente",
                    FechaEnvio = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar email a {request.Para}");
                return new EmailResponse
                {
                    Exitoso = false,
                    Mensaje = "Error al enviar el email",
                    Error = ex.Message,
                    FechaEnvio = DateTime.UtcNow
                };
            }
        }

        private async Task<string> CargarPlantillaAsync(string nombrePlantilla)
        {
            // Normalizar el nombre de la plantilla
            if (!nombrePlantilla.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                nombrePlantilla += ".html";
            }

            var rutaCompleta = Path.Combine(_rutaPlantillas, nombrePlantilla);
            _logger.LogInformation($"Buscando plantilla en: {rutaCompleta}");

            if (!File.Exists(rutaCompleta))
            {
                _logger.LogWarning($"Plantilla no encontrada: {rutaCompleta}");
                throw new FileNotFoundException($"La plantilla '{nombrePlantilla}' no existe");
            }

            _logger.LogInformation($"Cargando plantilla: {rutaCompleta}");
            return await File.ReadAllTextAsync(rutaCompleta);
        }

        private string ProcesarPlantilla(string plantilla, EmailRequest request)
        {
            // Reemplazar el contenido principal
            plantilla = plantilla.Replace("{{Contenido}}", request.Contenido);
            plantilla = plantilla.Replace("{{Asunto}}", request.Asunto);

            // Reemplazar variables personalizadas
            if (request.Variables != null)
            {
                foreach (var variable in request.Variables)
                {
                    plantilla = plantilla.Replace($"{{{{{variable.Key}}}}}", variable.Value);
                }
            }

            // Reemplazar variables de sistema
            plantilla = plantilla.Replace("{{Año}}", DateTime.Now.Year.ToString());
            plantilla = plantilla.Replace("{{Fecha}}", DateTime.Now.ToString("dd/MM/yyyy"));
            plantilla = plantilla.Replace("{{FechaHora}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            return plantilla;
        }
    }
}
