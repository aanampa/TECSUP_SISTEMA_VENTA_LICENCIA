namespace SistemaLicencias.Auth.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}
