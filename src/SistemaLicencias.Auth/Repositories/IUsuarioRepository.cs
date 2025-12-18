using SistemaLicencias.Auth.Models;

namespace SistemaLicencias.Auth.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorUsernameAsync(string username);
        Task<Usuario?> ObtenerPorIdAsync(int idUsuario);
        Task<int> CrearUsuarioAsync(Usuario usuario);
        Task<bool> ExisteUsernameAsync(string username);
        Task<List<Opcion>> ObtenerOpcionesPorUsuarioAsync(int idUsuario);
        Task AsignarOpcionesAsync(int idUsuario, List<int> opcionesIds);
    }
}
