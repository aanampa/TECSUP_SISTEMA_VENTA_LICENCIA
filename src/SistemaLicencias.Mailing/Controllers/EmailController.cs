using EmailAPI.Models;
using EmailAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Envía un email simple o usando una plantilla HTML
        /// </summary>
        /// <param name="request">Datos del email a enviar</param>
        /// <returns>Resultado del envío</returns>
        [HttpPost("enviar")]
        [ProducesResponseType(typeof(EmailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EnviarEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Para))
            {
                return BadRequest(new EmailResponse
                {
                    Exitoso = false,
                    Mensaje = "El campo 'Para' es obligatorio",
                    FechaEnvio = DateTime.UtcNow
                });
            }

            if (string.IsNullOrWhiteSpace(request.Asunto))
            {
                return BadRequest(new EmailResponse
                {
                    Exitoso = false,
                    Mensaje = "El campo 'Asunto' es obligatorio",
                    FechaEnvio = DateTime.UtcNow
                });
            }

            if (string.IsNullOrWhiteSpace(request.Contenido))
            {
                return BadRequest(new EmailResponse
                {
                    Exitoso = false,
                    Mensaje = "El campo 'Contenido' es obligatorio",
                    FechaEnvio = DateTime.UtcNow
                });
            }

            var resultado = await _emailService.EnviarEmailAsync(request);

            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, resultado);
            }
        }

        /// <summary>
        /// Endpoint de prueba para verificar que el API está funcionando
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                Estado = "Activo",
                Fecha = DateTime.UtcNow,
                Servicio = "Email API",
                Version = "1.0.0"
            });
        }

        /// <summary>
        /// Lista las plantillas disponibles
        /// </summary>
        [HttpGet("plantillas")]
        public IActionResult ListarPlantillas([FromServices] IWebHostEnvironment env)
        {
            try
            {
                var rutaPlantillas = Path.Combine(env.ContentRootPath, "Resources", "Templates");
                
                if (!Directory.Exists(rutaPlantillas))
                {
                    return Ok(new { Plantillas = new List<string>(), Mensaje = "No hay plantillas disponibles" });
                }

                var plantillas = Directory.GetFiles(rutaPlantillas, "*.html")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToList();

                return Ok(new { Plantillas = plantillas, Total = plantillas.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar plantillas");
                return StatusCode(500, new { Error = "Error al obtener la lista de plantillas" });
            }
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "Running",
                serverTime = DateTime.Now,
                service = "Mailing API" // Cambia esto según el proyecto
            });
        }

    }
}
