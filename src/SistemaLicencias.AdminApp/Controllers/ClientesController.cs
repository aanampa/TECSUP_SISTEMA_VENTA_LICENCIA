using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLicencias.AdminApp.Services;

namespace SistemaLicencias.AdminApp.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IAdminApiService _apiService;

        public ClientesController(IAdminApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _apiService.ObtenerClientesAsync();
            return View(clientes);
        }
    }
}
