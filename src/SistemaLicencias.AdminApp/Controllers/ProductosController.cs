using Microsoft.AspNetCore.Mvc;
using SistemaLicencias.AdminApp.Models.ViewModels;
using SistemaLicencias.AdminApp.Services;

namespace SistemaLicencias.AdminApp.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IAdminApiService _apiService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IAdminApiService apiService, ILogger<ProductosController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _apiService.ObtenerProductosAsync();
            return View(productos);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new ProductoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ProductoViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var resultado = await _apiService.CrearProductoAsync(modelo);

            if (resultado.Exito)
            {
                TempData["Mensaje"] = "Producto creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = resultado.Mensaje;
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _apiService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(ProductoViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var resultado = await _apiService.ActualizarProductoAsync(modelo);

            if (resultado.Exito)
            {
                TempData["Mensaje"] = "Producto actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = resultado.Mensaje;
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _apiService.EliminarProductoAsync(id);

            if (resultado.Exito)
            {
                TempData["Mensaje"] = "Producto eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = resultado.Mensaje;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
