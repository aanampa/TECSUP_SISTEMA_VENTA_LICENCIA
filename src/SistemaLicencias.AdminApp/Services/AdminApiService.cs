using SistemaLicencias.AdminApp.Models.ViewModels;
using SistemaLicencias.AdminApp.Models.DTOs;
using Newtonsoft.Json;
using System.Text;


namespace SistemaLicencias.AdminApp.Services
{
    public class AdminApiService: IAdminApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AdminApiService> _logger;

        public AdminApiService(HttpClient httpClient, IConfiguration configuration, ILogger<AdminApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        // ==================== AUTENTICACIÓN ====================
        public async Task<bool> ValidarCredencialesAsync(string usuario, string password)
        {
            try
            {
                var request = new { usuario, password };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Simulación temporal - en producción hacer POST a /api/Auth/Login
                _logger.LogInformation("Validando credenciales para usuario: {Usuario}", usuario);

                // Por ahora validación simple
                return usuario == "admin" && password == "Admin123!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar credenciales");
                return false;
            }
        }

        // ==================== PRODUCTOS ====================
        public async Task<List<ProductoViewModel>> ObtenerProductosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/ProductosApi");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ProductoViewModel>>(content) ?? new List<ProductoViewModel>();
                }
                return new List<ProductoViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return new List<ProductoViewModel>();
            }
        }

        public async Task<ProductoViewModel> ObtenerProductoPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/ProductosApi/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ProductoViewModel>(content);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                return null;
            }
        }

        public async Task<ApiResponse<int>> CrearProductoAsync(ProductoViewModel producto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/ProductosApi", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResponse<int>>(responseContent);
                }

                return new ApiResponse<int> { Exito = false, Mensaje = "Error al crear producto" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return new ApiResponse<int> { Exito = false, Mensaje = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> ActualizarProductoAsync(ProductoViewModel producto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/ProductosApi/{producto.IdProducto}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                }

                return new ApiResponse<bool> { Exito = false, Mensaje = "Error al actualizar producto" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto");
                return new ApiResponse<bool> { Exito = false, Mensaje = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> EliminarProductoAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/ProductosApi/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                }

                return new ApiResponse<bool> { Exito = false, Mensaje = "Error al eliminar producto" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto");
                return new ApiResponse<bool> { Exito = false, Mensaje = ex.Message };
            }
        }

        // ==================== PEDIDOS ====================
        public async Task<List<PedidoViewModel>> ObtenerPedidosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/PedidosApi");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<PedidoViewModel>>(content) ?? new List<PedidoViewModel>();
                }
                return new List<PedidoViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos");
                return new List<PedidoViewModel>();
            }
        }

        public async Task<PedidoViewModel> ObtenerPedidoPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/PedidosApi/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PedidoViewModel>(content);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedido {Id}", id);
                return null;
            }
        }

        public async Task<ApiResponse<bool>> ActualizarEstadoPedidoAsync(int id, ActualizarEstadoPedidoDto dto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/PedidosApi/{id}/estado", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                }

                return new ApiResponse<bool> { Exito = false, Mensaje = "Error al actualizar estado" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado del pedido");
                return new ApiResponse<bool> { Exito = false, Mensaje = ex.Message };
            }
        }

        // ==================== CLIENTES ====================
        public async Task<List<ClienteViewModel>> ObtenerClientesAsync()
        {
            try
            {
                // Por ahora retornar vacío - necesitaríamos endpoint en la API
                return new List<ClienteViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return new List<ClienteViewModel>();
            }
        }

        // ==================== LICENCIAS ====================
        public async Task<List<LicenciaViewModel>> ObtenerLicenciasAsync()
        {
            try
            {
                // Por ahora retornar vacío - necesitaríamos endpoint en la API
                return new List<LicenciaViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener licencias");
                return new List<LicenciaViewModel>();
            }
        }

        public async Task<List<LicenciaViewModel>> ObtenerLicenciasPorProductoAsync(int idProducto)
        {
            try
            {
                // Por ahora retornar vacío - necesitaríamos endpoint en la API
                return new List<LicenciaViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener licencias por producto");
                return new List<LicenciaViewModel>();
            }
        }

    }
}
