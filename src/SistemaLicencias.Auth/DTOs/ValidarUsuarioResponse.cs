using SistemaLicencias.Auth.Models;

namespace SistemaLicencias.Auth.DTOs
{
    public class ValidarUsuarioResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public UsuarioInfo? Usuario { get; set; }
        public List<Opcion>? Opciones { get; set; }
    }

    public class UsuarioInfo
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
