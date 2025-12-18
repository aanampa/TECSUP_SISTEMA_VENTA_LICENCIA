using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLicencias.AdminApp.Models.DTOs;
using SistemaLicencias.AdminApp.Services;

namespace SistemaLicencias.AdminApp.Controllers
{
    [Authorize]
    public class PedidosController : Controller
    {
        private readonly IAdminApiService _apiService;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(IAdminApiService apiService, ILogger<PedidosController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string estado = null)
        {
            var pedidos = await _apiService.ObtenerPedidosAsync();

            if (!string.IsNullOrEmpty(estado))
            {
                pedidos = pedidos.Where(p => p.EstadoPedido == estado).ToList();
            }

            ViewBag.EstadoFiltro = estado;
            return View(pedidos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var pedido = await _apiService.ObtenerPedidoPorIdAsync(id);
            if (pedido == null)
            {
                TempData["Error"] = "Pedido no encontrado";
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarEstado(int id, string estadoPago, string estadoPedido)
        {
            var dto = new ActualizarEstadoPedidoDto
            {
                EstadoPago = estadoPago,
                EstadoPedido = estadoPedido
            };

            var resultado = await _apiService.ActualizarEstadoPedidoAsync(id, dto);

            if (resultado.Exito)
            {
                TempData["Mensaje"] = "Estado actualizado exitosamente";
            }
            else
            {
                TempData["Error"] = resultado.Mensaje;
            }

            return RedirectToAction(nameof(Detalle), new { id });
        }
    }
}
