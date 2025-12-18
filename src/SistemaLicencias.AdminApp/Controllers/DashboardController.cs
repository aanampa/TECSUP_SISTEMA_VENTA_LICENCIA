using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLicencias.AdminApp.Models.ViewModels;
using SistemaLicencias.AdminApp.Services;

namespace SistemaLicencias.AdminApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IAdminApiService _apiService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IAdminApiService apiService, ILogger<DashboardController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _apiService.ObtenerPedidosAsync();
            var productos = await _apiService.ObtenerProductosAsync();

            var dashboard = new DashboardViewModel
            {
                Estadisticas = new EstadisticasViewModel
                {
                    VentasHoy = pedidos.Where(p => p.Fecha.Date == DateTime.Today).Sum(p => p.Total),
                    VentasMes = pedidos.Where(p => p.Fecha.Month == DateTime.Now.Month).Sum(p => p.Total),
                    PedidosHoy = pedidos.Count(p => p.Fecha.Date == DateTime.Today),
                    PedidosMes = pedidos.Count(p => p.Fecha.Month == DateTime.Now.Month),
                    TotalClientes = pedidos.Select(p => p.IdCliente).Distinct().Count(),
                    LicenciasDisponibles = productos.Sum(p => p.CantidadLicenciasDisponibles)
                },
                PedidosRecientes = pedidos.OrderByDescending(p => p.Fecha).Take(10).ToList(),
                ProductosMasVendidos = pedidos
                    .SelectMany(p => p.Detalles)
                    .GroupBy(d => d.NombreProducto)
                    .Select(g => new ProductoMasVendidoViewModel
                    {
                        NombreProducto = g.Key,
                        CantidadVendida = g.Sum(d => d.Cantidad),
                        TotalVentas = g.Sum(d => d.Subtotal)
                    })
                    .OrderByDescending(p => p.CantidadVendida)
                    .Take(5)
                    .ToList()
            };

            return View(dashboard);
        }
    }
}
