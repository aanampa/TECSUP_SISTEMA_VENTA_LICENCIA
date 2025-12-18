using Application.DTOs.Niubiz;
using Application.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PagoApiController : ControllerBase
    {
        private readonly INiubizService _niubizService;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ILogger<PagoApiController> _logger;

        public PagoApiController(
            INiubizService niubizService,
            IPedidoRepository pedidoRepository,
            ILogger<PagoApiController> logger)
        {
            _niubizService = niubizService;
            _pedidoRepository = pedidoRepository;
            _logger = logger;
        }

        /// <summary>
        /// Genera el token de sesión para Niubiz
        /// </summary>
        /// <param name="request">Datos para generar token</param>
        /// <returns>Token de sesión</returns>
        [HttpPost("generar-token")]
        [ProducesResponseType(typeof(GenerarTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GenerarTokenResponse>> GenerarToken([FromBody] GenerarTokenRequest request)
        {
            _logger.LogInformation("Ejecutando: generar-token");

            var json = JsonConvert.SerializeObject(request);
            _logger.LogInformation($"- request {json}");

            if (!ModelState.IsValid)
                return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState });

            //var remoteIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            //if (remoteIp == "::1") { remoteIp = "127.0.0.1"; }

            var resultado = await _niubizService.GenerarTokenSesionAsync2(request);

            if (resultado.Exito)
                return Ok(resultado);

            return BadRequest(resultado);
        }

        /// <summary>
        /// Confirma el pago después de la autorización de Niubiz
        /// </summary>
        /// <param name="request">Datos de confirmación</param>
        /// <returns>Resultado de la confirmación</returns>
        [HttpPost("confirmar")]
        [ProducesResponseType(typeof(ConfirmarPagoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConfirmarPagoResponse>> ConfirmarPago([FromBody] ConfirmarPagoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState });

            var resultado = await _niubizService.AutorizarTransaccionAsync2(request);

            if (resultado.Exito)
            {
                // Actualizar estado del pedido
                var pedido = await _pedidoRepository.ObtenerPorIdAsync(request.IdPedido);
                if (pedido != null)
                {
                    pedido.EstadoPago = "Pagado";
                    pedido.EstadoPedido = "Completado";
                    await _pedidoRepository.ActualizarAsync(pedido);
                }

                return Ok(resultado);
            }

            return BadRequest(resultado);
        }


        [HttpGet("get-token")]
        [ProducesResponseType(typeof(ConfirmarPagoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ApiResponse> GetToken()
        {

            string token = await _niubizService.ObtenerAccessTokenAsync();

            ApiResponse response = new ApiResponse
            {
                Data = token,
                Status = true
            };

            return response;
        }
    }
}
