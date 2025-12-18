using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> ObtenerTodosAsync();
        Task<ProductoDto> ObtenerPorIdAsync(int id);
        Task<ResultadoDto<int>> CrearAsync(ProductoDto productoDto);
        Task<ResultadoDto<bool>> ActualizarAsync(ProductoDto productoDto);
    }

    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(IProductoRepository productoRepository, ILogger<ProductoService> logger)
        {
            _productoRepository = productoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerTodosAsync()
        {
            try
            {
                var productos = await _productoRepository.ObtenerTodosAsync();
                return productos.Select(p => new ProductoDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Imagen = p.Imagen,
                    Precio = p.Precio,
                    CantidadLicenciasDisponibles = p.CantidadLicenciasDisponibles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return Enumerable.Empty<ProductoDto>();
            }
        }

        public async Task<ProductoDto> ObtenerPorIdAsync(int id)
        {
            try
            {
                var producto = await _productoRepository.ObtenerPorIdAsync(id);
                if (producto == null) return null;

                return new ProductoDto
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre,
                    Imagen = producto.Imagen,
                    Precio = producto.Precio,
                    CantidadLicenciasDisponibles = producto.CantidadLicenciasDisponibles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                return null;
            }
        }

        public async Task<ResultadoDto<int>> CrearAsync(ProductoDto productoDto)
        {
            try
            {
                var producto = new Producto
                {
                    Nombre = productoDto.Nombre,
                    Imagen = productoDto.Imagen,
                    Precio = productoDto.Precio,
                    CantidadLicenciasDisponibles = productoDto.CantidadLicenciasDisponibles,
                    Activo = true
                };

                var id = await _productoRepository.CrearAsync(producto);
                return ResultadoDto<int>.ExitoResultado(id, "Producto creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return ResultadoDto<int>.ErrorResultado("Error al crear el producto");
            }
        }

        public async Task<ResultadoDto<bool>> ActualizarAsync(ProductoDto productoDto)
        {
            try
            {
                var producto = new Producto
                {
                    IdProducto = productoDto.IdProducto,
                    Nombre = productoDto.Nombre,
                    Imagen = productoDto.Imagen,
                    Precio = productoDto.Precio,
                    CantidadLicenciasDisponibles = productoDto.CantidadLicenciasDisponibles,
                    Activo = true
                };

                var resultado = await _productoRepository.ActualizarAsync(producto);
                return resultado
                    ? ResultadoDto<bool>.ExitoResultado(true, "Producto actualizado exitosamente")
                    : ResultadoDto<bool>.ErrorResultado("No se pudo actualizar el producto");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto");
                return ResultadoDto<bool>.ErrorResultado("Error al actualizar el producto");
            }
        }
    }

}
