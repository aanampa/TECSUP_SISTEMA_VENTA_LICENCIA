using SistemaLicencias.Auth.DTOs;
using SistemaLicencias.Auth.Models;
using SistemaLicencias.Auth.Repositories;

namespace SistemaLicencias.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ValidarUsuarioResponse> ValidarUsuarioAsync(ValidarUsuarioRequest request)
        {
            try
            {
                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return new ValidarUsuarioResponse
                    {
                        Exito = false,
                        Mensaje = "Username y Password son obligatorios"
                    };
                }

                // Buscar usuario por username
                var usuario = await _usuarioRepository.ObtenerPorUsernameAsync(request.Username);

                if (usuario == null)
                {
                    return new ValidarUsuarioResponse
                    {
                        Exito = false,
                        Mensaje = "Usuario no encontrado"
                    };
                }

                // Validar password (en producción usar hash)
                if (usuario.Password != request.Password)
                {
                    return new ValidarUsuarioResponse
                    {
                        Exito = false,
                        Mensaje = "Credenciales incorrectas"
                    };
                }

                // Obtener opciones del usuario
                var opciones = await _usuarioRepository.ObtenerOpcionesPorUsuarioAsync(usuario.IdUsuario);

                return new ValidarUsuarioResponse
                {
                    Exito = true,
                    Mensaje = "Usuario validado correctamente",
                    Usuario = new UsuarioInfo
                    {
                        IdUsuario = usuario.IdUsuario,
                        Username = usuario.Username,
                        Nombre = usuario.Nombre,
                        Email = usuario.Email
                    },
                    Opciones = opciones
                };
            }
            catch (Exception ex)
            {
                return new ValidarUsuarioResponse
                {
                    Exito = false,
                    Mensaje = $"Error al validar usuario: {ex.Message}"
                };
            }
        }

        public async Task<RegistrarUsuarioResponse> RegistrarUsuarioAsync(RegistrarUsuarioRequest request)
        {
            try
            {
                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.Nombre) ||
                    string.IsNullOrWhiteSpace(request.Email))
                {
                    return new RegistrarUsuarioResponse
                    {
                        Exito = false,
                        Mensaje = "Todos los campos son obligatorios"
                    };
                }

                // Validar si el username ya existe
                var existe = await _usuarioRepository.ExisteUsernameAsync(request.Username);
                if (existe)
                {
                    return new RegistrarUsuarioResponse
                    {
                        Exito = false,
                        Mensaje = "El username ya existe"
                    };
                }

                // Crear nuevo usuario
                var nuevoUsuario = new Usuario
                {
                    Username = request.Username,
                    Password = request.Password, // En producción usar hash
                    Nombre = request.Nombre,
                    Email = request.Email,
                    Activo = true
                };

                var idUsuario = await _usuarioRepository.CrearUsuarioAsync(nuevoUsuario);

                // Asignar opciones al perfil del usuario
                if (request.OpcionesIds != null && request.OpcionesIds.Any())
                {
                    await _usuarioRepository.AsignarOpcionesAsync(idUsuario, request.OpcionesIds);
                }

                return new RegistrarUsuarioResponse
                {
                    Exito = true,
                    Mensaje = "Usuario registrado correctamente",
                    IdUsuario = idUsuario
                };
            }
            catch (Exception ex)
            {
                return new RegistrarUsuarioResponse
                {
                    Exito = false,
                    Mensaje = $"Error al registrar usuario: {ex.Message}"
                };
            }
        }
    }
}
