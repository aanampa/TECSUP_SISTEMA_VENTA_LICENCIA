using Microsoft.AspNetCore.Mvc;
using SistemaLicencias.Auth.DTOs;
using SistemaLicencias.Auth.Services;

namespace SistemaLicencias.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Valida un usuario y devuelve las opciones a las que tiene acceso
        /// </summary>
        /// <param name="request">Credenciales del usuario</param>
        /// <returns>Información del usuario y sus opciones</returns>
        [HttpPost("validar")]
        public async Task<ActionResult<ValidarUsuarioResponse>> ValidarUsuario([FromBody] ValidarUsuarioRequest request)
        {
            _logger.LogInformation("Validando usuario: {Username}", request.Username);
            
            var response = await _authService.ValidarUsuarioAsync(request);
            
            if (!response.Exito)
            {
                _logger.LogWarning("Validación fallida para usuario: {Username}", request.Username);
                return BadRequest(response);
            }

            _logger.LogInformation("Usuario validado exitosamente: {Username}", request.Username);
            return Ok(response);
        }

        /// <summary>
        /// Registra un nuevo usuario y asigna opciones a su perfil
        /// </summary>
        /// <param name="request">Datos del nuevo usuario</param>
        /// <returns>Resultado del registro</returns>
        [HttpPost("registrar")]
        public async Task<ActionResult<RegistrarUsuarioResponse>> RegistrarUsuario([FromBody] RegistrarUsuarioRequest request)
        {
            _logger.LogInformation("Registrando nuevo usuario: {Username}", request.Username);
            
            var response = await _authService.RegistrarUsuarioAsync(request);
            
            if (!response.Exito)
            {
                _logger.LogWarning("Registro fallido para usuario: {Username}", request.Username);
                return BadRequest(response);
            }

            _logger.LogInformation("Usuario registrado exitosamente: {Username} con ID: {IdUsuario}", 
                request.Username, response.IdUsuario);
            
            return CreatedAtAction(nameof(RegistrarUsuario), new { id = response.IdUsuario }, response);
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "Running",
                serverTime = DateTime.Now,
                service = "Auth API" // Cambia esto según el proyecto
            });
        }
    }
}
