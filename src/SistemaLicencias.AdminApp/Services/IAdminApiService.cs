using SistemaLicencias.AdminApp.Models.DTOs;
using SistemaLicencias.AdminApp.Models.ViewModels;

namespace SistemaLicencias.AdminApp.Services
{
    public interface IAdminApiService
    {
        // Autenticación
        Task<bool> ValidarCredencialesAsync(string usuario, string password);

        // Productos
        Task<List<ProductoViewModel>> ObtenerProductosAsync();
        Task<ProductoViewModel> ObtenerProductoPorIdAsync(int id);
        Task<ApiResponse<int>> CrearProductoAsync(ProductoViewModel producto);
        Task<ApiResponse<bool>> ActualizarProductoAsync(ProductoViewModel producto);
        Task<ApiResponse<bool>> EliminarProductoAsync(int id);

        // Pedidos
        Task<List<PedidoViewModel>> ObtenerPedidosAsync();
        Task<PedidoViewModel> ObtenerPedidoPorIdAsync(int id);
        Task<ApiResponse<bool>> ActualizarEstadoPedidoAsync(int id, ActualizarEstadoPedidoDto dto);

        // Clientes
        Task<List<ClienteViewModel>> ObtenerClientesAsync();

        // Licencias
        Task<List<LicenciaViewModel>> ObtenerLicenciasAsync();
        Task<List<LicenciaViewModel>> ObtenerLicenciasPorProductoAsync(int idProducto);
    }
}
