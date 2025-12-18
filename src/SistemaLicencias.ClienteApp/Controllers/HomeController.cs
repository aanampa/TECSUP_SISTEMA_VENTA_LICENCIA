using Microsoft.AspNetCore.Mvc;
using SistemaLicencias.ClienteApp.Services;

namespace SistemaLicencias.ClienteApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILicenciasApiService _apiService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILicenciasApiService apiService, ILogger<HomeController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _apiService.ObtenerProductosAsync();
            return View(productos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

    }
}
