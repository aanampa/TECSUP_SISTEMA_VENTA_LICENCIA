namespace SistemaLicencias.Auth.DTOs
{
    public class RegistrarUsuarioResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
    }
}
