using System.Data;

namespace SistemaLicencias.Auth.Data
{
    public interface IDbContext
    {
        IDbConnection CreateConnection();
    }
}
