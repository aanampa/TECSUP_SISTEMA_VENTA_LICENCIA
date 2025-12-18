using Dapper;
using SistemaLicencias.Auth.Data;
using SistemaLicencias.Auth.Models;

namespace SistemaLicencias.Auth.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbContext _dbContext;

        public UsuarioRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario?> ObtenerPorUsernameAsync(string username)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT IdUsuario, Username, Password, Nombre, Email, Activo 
                FROM Usuarios 
                WHERE Username = @Username AND Activo = 1";
            
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Username = username });
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int idUsuario)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT IdUsuario, Username, Password, Nombre, Email, Activo 
                FROM Usuarios 
                WHERE IdUsuario = @IdUsuario";
            
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { IdUsuario = idUsuario });
        }

        public async Task<int> CrearUsuarioAsync(Usuario usuario)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO Usuarios (Username, Password, Nombre, Email, Activo)
                VALUES (@Username, @Password, @Nombre, @Email, @Activo);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            
            return await connection.ExecuteScalarAsync<int>(sql, usuario);
        }

        public async Task<bool> ExisteUsernameAsync(string username)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT COUNT(1) FROM Usuarios WHERE Username = @Username";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Username = username });
            return count > 0;
        }

        public async Task<List<Opcion>> ObtenerOpcionesPorUsuarioAsync(int idUsuario)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT o.IdOpcion, o.NombreOpcion, o.UrlOpcion
                FROM Opcion o
                INNER JOIN Perfil p ON o.IdOpcion = p.IdOpcion
                WHERE p.IdUsuario = @IdUsuario";
            
            var opciones = await connection.QueryAsync<Opcion>(sql, new { IdUsuario = idUsuario });
            return opciones.ToList();
        }

        public async Task AsignarOpcionesAsync(int idUsuario, List<int> opcionesIds)
        {
            using var connection = _dbContext.CreateConnection();
            
            // Eliminar opciones existentes del usuario
            var deleteSql = "DELETE FROM Perfil WHERE IdUsuario = @IdUsuario";
            await connection.ExecuteAsync(deleteSql, new { IdUsuario = idUsuario });
            
            // Insertar nuevas opciones
            if (opcionesIds.Any())
            {
                var insertSql = "INSERT INTO Perfil (IdUsuario, IdOpcion) VALUES (@IdUsuario, @IdOpcion)";
                foreach (var opcionId in opcionesIds)
                {
                    await connection.ExecuteAsync(insertSql, new { IdUsuario = idUsuario, IdOpcion = opcionId });
                }
            }
        }
    }
}
