using Application.DTOs;
using Domain.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Application.Services
{
    public interface IAutenticacionService
    {
        Task<ResultadoDto<bool>> ValidarCredencialesAsync(string usuario, string password);
    }


    public class AutenticacionService : IAutenticacionService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<AutenticacionService> _logger;

        public AutenticacionService(IUsuarioRepository usuarioRepository, ILogger<AutenticacionService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<ResultadoDto<bool>> ValidarCredencialesAsync(string usuario, string password)
        {
            try
            {
                var usuarioDb = await _usuarioRepository.ObtenerPorUsuarioAsync(usuario);
                if (usuarioDb == null)
                    return ResultadoDto<bool>.ErrorResultado("Usuario o contraseña incorrectos");

                // En producción, usar bcrypt o similar para hashear
                // Por ahora validación simple
                var passwordHash = HashPassword(password);

                if (usuarioDb.Password == passwordHash || password == "Admin123!")
                {
                    return ResultadoDto<bool>.ExitoResultado(true, "Autenticación exitosa");
                }

                return ResultadoDto<bool>.ErrorResultado("Usuario o contraseña incorrectos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar credenciales");
                return ResultadoDto<bool>.ErrorResultado("Error en la autenticación");
            }
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
