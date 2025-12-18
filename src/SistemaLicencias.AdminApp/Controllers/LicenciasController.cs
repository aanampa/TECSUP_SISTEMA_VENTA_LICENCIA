using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLicencias.AdminApp.Services;

namespace SistemaLicencias.AdminApp.Controllers
{
    [Authorize]
    public class LicenciasController : Controller
    {
        private readonly IAdminApiService _apiService;

        public LicenciasController(IAdminApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(int? idProducto = null)
        {
            var licencias = idProducto.HasValue
                ? await _apiService.ObtenerLicenciasPorProductoAsync(idProducto.Value)
                : await _apiService.ObtenerLicenciasAsync();

            ViewBag.IdProductoFiltro = idProducto;
            return View(licencias);
        }
    }
}
