using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SistemaLicencias.AdminApp.Models.ViewModels;
using SistemaLicencias.AdminApp.Services;
using System.Security.Claims;

namespace SistemaLicencias.AdminApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAdminApiService _apiService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAdminApiService apiService, ILogger<AuthController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var esValido = await _apiService.ValidarCredencialesAsync(model.Usuario, model.Password);

            if (esValido)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Usuario),
                new Claim(ClaimTypes.Role, "Administrador")
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RecordarMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Usuario {Usuario} inició sesión", model.Usuario);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Usuario {Usuario} cerró sesión", User.Identity.Name);
            return RedirectToAction("Login");
        }
    }
}
