using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosApiController : ControllerBase
    {

        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosApiController> _logger;

        public ProductosApiController(IProductoService productoService, ILogger<ProductosApiController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los productos disponibles
        /// </summary>
        /// <returns>Lista de productos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            _logger.LogError("Ejecutando GetProductos");

            var productos = await _productoService.ObtenerTodosAsync();
            return Ok(productos);
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Producto encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            return Ok(producto);
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "Running",
                serverTime = DateTime.Now,
                service = "Producto API" // Cambia esto según el proyecto
            });
        }

    }
}
