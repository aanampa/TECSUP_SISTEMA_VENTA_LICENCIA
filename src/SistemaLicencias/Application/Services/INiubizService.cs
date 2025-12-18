using Application.DTOs.Niubiz;

namespace Application.Services
{
    public interface INiubizService
    {
        Task<GenerarTokenResponse> GenerarTokenSesionAsync(GenerarTokenRequest request);
        Task<GenerarTokenResponse> GenerarTokenSesionAsync2(GenerarTokenRequest request);
        Task<ConfirmarPagoResponse> AutorizarTransaccionAsync(ConfirmarPagoRequest request);
        Task<ConfirmarPagoResponse> AutorizarTransaccionAsync2(ConfirmarPagoRequest request);
        Task<string> ObtenerAccessTokenAsync();
        Task<string> ObtenerAccessTokenAsync2();
    }


}
