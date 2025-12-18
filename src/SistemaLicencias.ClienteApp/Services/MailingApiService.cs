using Newtonsoft.Json;
using SistemaLicencias.ClienteApp.Models;
using System.Text;

namespace SistemaLicencias.ClienteApp.Services
{
    public class MailingApiService : IMailingApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MailingApiService> _logger;
        private readonly string _baseMailingUrl;

        public MailingApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<MailingApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseMailingUrl = configuration["ApiSettings:BaseMailingUrl"];
            _httpClient.BaseAddress = new Uri(_baseMailingUrl);
        }


        public async Task<ApiResponse<EmailResponse>> EnviarEmailAsync(string email, string nombre, string asunto, string mensaje)
        {

            EmailRequest emailRequest = new EmailRequest();
            emailRequest.Para = email;
            emailRequest.Asunto = asunto;
            emailRequest.Contenido = mensaje;
            emailRequest.NombrePlantilla = "notificacion";
            emailRequest.Variables = new Dictionary<string, string> { { "Nombre", nombre } };

            try
            {


                var json = JsonConvert.SerializeObject(emailRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Email/enviar", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonConvert.DeserializeObject<EmailResponse>(responseContent);
                    return new ApiResponse<EmailResponse>
                    {
                        Exito = true,
                    };
                }

                return new ApiResponse<EmailResponse>
                {
                    Exito = false,
                    Mensaje = $"Error al crear el pedido: {response.StatusCode}"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido en la API");
                return new ApiResponse<EmailResponse>
                {
                    Exito = false,
                    Mensaje = "Error al comunicarse con el servidor"
                };
            }

          
            
        }
    }
}
