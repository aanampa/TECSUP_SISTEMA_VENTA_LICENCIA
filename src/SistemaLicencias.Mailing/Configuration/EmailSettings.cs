namespace EmailAPI.Configuration
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public bool UsarSSL { get; set; }
        public string EmailRemitente { get; set; } = string.Empty;
        public string NombreRemitente { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
    }
}
