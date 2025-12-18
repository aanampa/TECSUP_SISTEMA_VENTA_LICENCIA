using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public interface IPedidoService
    {
        Task<ResultadoDto<int>> CrearPedidoAsync(CrearPedidoDto pedidoDto);
        Task<IEnumerable<PedidoDto>> ObtenerTodosAsync();
        Task<ResultadoDto<bool>> ActualizarEstadoAsync(int idPedido, string estadoPago, string estadoPedido);
    }

    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IDetallePedidoRepository _detallePedidoRepository;
        private readonly ILicenciaRepository _licenciaRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IClienteRepository clienteRepository,
            IProductoRepository productoRepository,
            IDetallePedidoRepository detallePedidoRepository,
            ILicenciaRepository licenciaRepository,
            IEmailService emailService,
            ILogger<PedidoService> logger)
        {
            _pedidoRepository = pedidoRepository;
            _clienteRepository = clienteRepository;
            _productoRepository = productoRepository;
            _detallePedidoRepository = detallePedidoRepository;
            _licenciaRepository = licenciaRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ResultadoDto<int>> CrearPedidoAsync(CrearPedidoDto pedidoDto)
        {
            try
            {
                _logger.LogInformation("Iniciando creación de pedido");

                // Validar producto y disponibilidad
                var producto = await _productoRepository.ObtenerPorIdAsync(pedidoDto.IdProducto);
                if (producto == null)
                    return ResultadoDto<int>.ErrorResultado("Producto no encontrado");

                if (producto.CantidadLicenciasDisponibles < pedidoDto.Cantidad)
                    return ResultadoDto<int>.ErrorResultado("No hay suficientes licencias disponibles");

                // Obtener o crear cliente
                var cliente = await _clienteRepository.ObtenerPorDocumentoAsync(pedidoDto.Cliente.Documento);
                if (cliente == null)
                {
                    cliente = new Cliente
                    {
                        Documento = pedidoDto.Cliente.Documento,
                        Nombre = pedidoDto.Cliente.Nombre,
                        Email = pedidoDto.Cliente.Email
                    };
                    cliente.IdCliente = await _clienteRepository.CrearAsync(cliente);
                }

                // Calcular totales
                decimal subtotal = producto.Precio * pedidoDto.Cantidad;
                decimal igv = subtotal * 0.18m;
                decimal total = subtotal + igv;

                // Crear pedido
                var pedido = new Pedido
                {
                    IdCliente = cliente.IdCliente,
                    Fecha = DateTime.Now,
                    IGV = igv,
                    Total = total,
                    EstadoPago = "Pagado",
                    EstadoPedido = "Completado"
                };

                int idPedido = await _pedidoRepository.CrearAsync(pedido);

                // Crear detalle
                var detalle = new DetallePedido
                {
                    IdPedido = idPedido,
                    IdProducto = producto.IdProducto,
                    Cantidad = pedidoDto.Cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = subtotal
                };
                await _detallePedidoRepository.CrearAsync(detalle);

                // Asignar licencias
                await _licenciaRepository.AsignarLicenciasPedidoAsync(idPedido, producto.IdProducto, pedidoDto.Cantidad);

                // Obtener licencias asignadas
                var licencias = await _licenciaRepository.ObtenerPorPedidoAsync(idPedido);
                var codigosLicencia = licencias.Select(l => l.CodigoLicencia).ToList();

                // Enviar email
                //await _emailService.EnviarEmailLicenciasAsync(
                //    cliente.Email,
                //    cliente.Nombre,
                //    codigosLicencia,
                //    producto.Nombre
                //);

                // Actualizar cantidad disponible
                producto.CantidadLicenciasDisponibles -= pedidoDto.Cantidad;
                await _productoRepository.ActualizarAsync(producto);

                _logger.LogInformation("Pedido {IdPedido} creado exitosamente", idPedido);
                return ResultadoDto<int>.ExitoResultado(idPedido, "Pedido creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido");
                return ResultadoDto<int>.ErrorResultado("Error al procesar el pedido");
            }
        }

        public async Task<IEnumerable<PedidoDto>> ObtenerTodosAsync()
        {
            try
            {
                var pedidos = await _pedidoRepository.ObtenerPedidosConDetallesAsync();

                return pedidos.Select(p => new PedidoDto
                {
                    IdPedido = p.IdPedido,
                    IdCliente = p.IdCliente,
                    NombreCliente = p.Cliente?.Nombre,
                    EmailCliente = p.Cliente?.Email,
                    Fecha = p.Fecha,
                    IGV = p.IGV,
                    Total = p.Total,
                    EstadoPago = p.EstadoPago,
                    EstadoPedido = p.EstadoPedido,
                    Detalles = p.Detalles?.Select(d => new DetallePedidoDto
                    {
                        IdProducto = d.IdProducto,
                        NombreProducto = d.Producto?.Nombre,
                        Cantidad = d.Cantidad,
                        Precio = d.PrecioUnitario
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos");
                return Enumerable.Empty<PedidoDto>();
            }
        }

        public async Task<ResultadoDto<bool>> ActualizarEstadoAsync(int idPedido, string estadoPago, string estadoPedido)
        {
            try
            {
                var pedido = await _pedidoRepository.ObtenerPorIdAsync(idPedido);
                if (pedido == null)
                    return ResultadoDto<bool>.ErrorResultado("Pedido no encontrado");

                pedido.EstadoPago = estadoPago;
                pedido.EstadoPedido = estadoPedido;

                var resultado = await _pedidoRepository.ActualizarAsync(pedido);
                return resultado
                    ? ResultadoDto<bool>.ExitoResultado(true, "Estado actualizado exitosamente")
                    : ResultadoDto<bool>.ErrorResultado("No se pudo actualizar el estado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado del pedido");
                return ResultadoDto<bool>.ErrorResultado("Error al actualizar el estado");
            }
        }
    }

}
