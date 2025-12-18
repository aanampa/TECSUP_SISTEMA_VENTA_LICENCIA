namespace SistemaLicencias.Auth.DTOs
{
    public class RegistrarUsuarioRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<int> OpcionesIds { get; set; } = new List<int>();
    }
}
