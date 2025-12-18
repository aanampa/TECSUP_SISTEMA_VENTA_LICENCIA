using Newtonsoft.Json;
using SistemaLicencias.ClienteApp.Models;
using System.Text;

namespace SistemaLicencias.ClienteApp.Services
{

    public class LicenciasApiService : ILicenciasApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LicenciasApiService> _logger;
        private readonly string _baseUrl;

        public LicenciasApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<LicenciasApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<List<ProductoViewModel>> ObtenerProductosAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo productos de la API");

                var response = await _httpClient.GetAsync("/api/ProductosApi");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(content);

                    _logger.LogInformation("Productos obtenidos: {Count}", productos?.Count ?? 0);
                    return productos ?? new List<ProductoViewModel>();
                }

                _logger.LogWarning("Error al obtener productos. Status: {StatusCode}", response.StatusCode);
                return new List<ProductoViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos de la API");
                return new List<ProductoViewModel>();
            }
        }

        public async Task<ProductoViewModel> ObtenerProductoPorIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo producto {Id} de la API", id);

                var response = await _httpClient.GetAsync($"/api/ProductosApi/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var producto = JsonConvert.DeserializeObject<ProductoViewModel>(content);

                    _logger.LogInformation("Producto obtenido: {Nombre}", producto?.Nombre);
                    return producto;
                }

                _logger.LogWarning("Producto {Id} no encontrado. Status: {StatusCode}", id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id} de la API", id);
                return null;
            }
        }

        public async Task<ApiResponse<int>> CrearPedidoAsync(CrearPedidoRequest pedido)
        {
            try
            {
                _logger.LogInformation("Creando pedido para producto {IdProducto}", pedido.IdProducto);

                var json = JsonConvert.SerializeObject(pedido);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/PedidosApi", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonConvert.DeserializeObject<ApiResponse<int>>(responseContent);
                    _logger.LogInformation("Pedido creado exitosamente: {IdPedido}", resultado?.Datos);
                    return resultado;
                }

                _logger.LogWarning("Error al crear pedido. Status: {StatusCode}", response.StatusCode);
                return new ApiResponse<int>
                {
                    Exito = false,
                    Mensaje = $"Error al crear el pedido: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido en la API");
                return new ApiResponse<int>
                {
                    Exito = false,
                    Mensaje = "Error al comunicarse con el servidor"
                };
            }
        }

        public async Task<GenerarTokenNiubizResponse> GenerarTokenNiubizAsync(GenerarTokenNiubizRequest request)
        {
            _logger.LogInformation($"Ejecutando: GenerarTokenNiubizAsync()");

            try
            {
                _logger.LogInformation("Generando token Niubiz para pedido {IdPedido}", request.IdPedido);

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation($"- json: {json}");

                var response = await _httpClient.PostAsync("/api/PagoApi/generar-token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonConvert.DeserializeObject<GenerarTokenNiubizResponse>(responseContent);
                    _logger.LogInformation("Token Niubiz generado: {SessionToken}", resultado?.SessionToken);
                    return resultado;
                }

                _logger.LogWarning("Error al generar token Niubiz. Status: {StatusCode}", response.StatusCode);
                return new GenerarTokenNiubizResponse
                {
                    Exito = false,
                    Mensaje = $"Error al generar token: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar token Niubiz");
                return new GenerarTokenNiubizResponse
                {
                    Exito = false,
                    Mensaje = "Error al comunicarse con el servidor"
                };
            }
        }

        public async Task<ConfirmarPagoNiubizResponse> ConfirmarPagoNiubizAsync(ConfirmarPagoNiubizRequest request)
        {
            try
            {
                _logger.LogInformation("Confirmando pago Niubiz para pedido {IdPedido}", request.IdPedido);

                //request.Amount = (decimal)59.99;

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/PagoApi/confirmar", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonConvert.DeserializeObject<ConfirmarPagoNiubizResponse>(responseContent);
                    _logger.LogInformation("Pago confirmado: {TransactionId}", resultado?.TransactionId);
                    return resultado;
                }

                _logger.LogWarning("Error al confirmar pago. Status: {StatusCode}", response.StatusCode);
                return new ConfirmarPagoNiubizResponse
                {
                    Exito = false,
                    Mensaje = $"Error al confirmar pago: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar pago Niubiz");
                return new ConfirmarPagoNiubizResponse
                {
                    Exito = false,
                    Mensaje = "Error al comunicarse con el servidor"
                };
            }
        }
    }

}
