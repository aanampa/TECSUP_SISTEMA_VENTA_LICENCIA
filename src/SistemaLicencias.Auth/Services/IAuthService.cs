using SistemaLicencias.Auth.DTOs;

namespace SistemaLicencias.Auth.Services
{
    public interface IAuthService
    {
        Task<ValidarUsuarioResponse> ValidarUsuarioAsync(ValidarUsuarioRequest request);
        Task<RegistrarUsuarioResponse> RegistrarUsuarioAsync(RegistrarUsuarioRequest request);
    }
}
