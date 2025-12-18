namespace EmailAPI.Models
{
    public class EmailRequest
    {
        /// <summary>
        /// Dirección de email del destinatario
        /// </summary>
        public string Para { get; set; } = string.Empty;

        /// <summary>
        /// Asunto del email
        /// </summary>
        public string Asunto { get; set; } = string.Empty;

        /// <summary>
        /// Contenido del email (puede ser texto plano o variables para la plantilla)
        /// </summary>
        public string Contenido { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la plantilla HTML a utilizar (opcional)
        /// </summary>
        public string? NombrePlantilla { get; set; }

        /// <summary>
        /// Variables adicionales para reemplazar en la plantilla
        /// </summary>
        public Dictionary<string, string>? Variables { get; set; }

        /// <summary>
        /// Lista de destinatarios adicionales (CC)
        /// </summary>
        public List<string>? Cc { get; set; }

        /// <summary>
        /// Lista de destinatarios ocultos (BCC)
        /// </summary>
        public List<string>? Bcc { get; set; }
    }
}
