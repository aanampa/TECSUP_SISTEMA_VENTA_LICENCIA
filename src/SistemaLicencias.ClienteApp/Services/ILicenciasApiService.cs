using SistemaLicencias.ClienteApp.Models;

namespace SistemaLicencias.ClienteApp.Services
{
    public interface ILicenciasApiService
    {
        Task<List<ProductoViewModel>> ObtenerProductosAsync();
        Task<ProductoViewModel> ObtenerProductoPorIdAsync(int id);
        Task<ApiResponse<int>> CrearPedidoAsync(CrearPedidoRequest pedido);

        Task<GenerarTokenNiubizResponse> GenerarTokenNiubizAsync(GenerarTokenNiubizRequest request);
        Task<ConfirmarPagoNiubizResponse> ConfirmarPagoNiubizAsync(ConfirmarPagoNiubizRequest request);
    }

}
