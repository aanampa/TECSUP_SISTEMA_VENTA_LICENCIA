using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosApiController : ControllerBase
    {

        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosApiController> _logger;

        public PedidosApiController(IPedidoService pedidoService, ILogger<PedidosApiController> logger)
        {
            _pedidoService = pedidoService;
            _logger = logger;
        }

        /// <summary>
        /// Crea un nuevo pedido de licencias
        /// </summary>
        /// <param name="pedidoDto">Datos del pedido</param>
        /// <returns>ID del pedido creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResultadoDto<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResultadoDto<int>>> CrearPedido([FromBody] CrearPedidoDto pedidoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState });

            var resultado = await _pedidoService.CrearPedidoAsync(pedidoDto);

            if (resultado.Exito)
                return Ok(resultado);

            return BadRequest(resultado);
        }

        /// <summary>
        /// Obtiene todos los pedidos
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PedidoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> GetPedidos()
        {
            var pedidos = await _pedidoService.ObtenerTodosAsync();
            return Ok(pedidos);
        }

        /// <summary>
        /// Actualiza el estado de un pedido
        /// </summary>
        /// <param name="id">ID del pedido</param>
        /// <param name="dto">Datos de actualización</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}/estado")]
        [ProducesResponseType(typeof(ResultadoDto<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResultadoDto<bool>>> ActualizarEstado(
            int id,
            [FromBody] ActualizarEstadoPedidoDto dto)
        {
            var resultado = await _pedidoService.ActualizarEstadoAsync(id, dto.EstadoPago, dto.EstadoPedido);

            if (resultado.Exito)
                return Ok(resultado);

            return BadRequest(resultado);
        }
    }
}
