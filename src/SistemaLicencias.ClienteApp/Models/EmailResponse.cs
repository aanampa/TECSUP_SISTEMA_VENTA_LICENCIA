namespace SistemaLicencias.ClienteApp.Models
{
    public class EmailResponse
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public string? Error { get; set; }
    }
}
