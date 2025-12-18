using SistemaLicencias.ClienteApp.Models;

namespace SistemaLicencias.ClienteApp.Services
{
    public interface IMailingApiService
    {
        Task<ApiResponse<EmailResponse>> EnviarEmailAsync(string email, string nombre, string asunto, string mensaje);
    }
}
