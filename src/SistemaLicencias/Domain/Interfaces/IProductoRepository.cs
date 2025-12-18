using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{

    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<Producto> ObtenerPorIdAsync(int id);
        Task<int> CrearAsync(Producto producto);
        Task<bool> ActualizarAsync(Producto producto);
        Task<bool> EliminarAsync(int id);
    }


    public interface IClienteRepository
    {
        Task<Cliente> ObtenerPorDocumentoAsync(string documento);
        Task<Cliente> ObtenerPorIdAsync(int id);
        Task<int> CrearAsync(Cliente cliente);
    }

    public interface IPedidoRepository
    {
        Task<IEnumerable<Pedido>> ObtenerTodosAsync();
        Task<Pedido> ObtenerPorIdAsync(int id);
        Task<int> CrearAsync(Pedido pedido);
        Task<bool> ActualizarAsync(Pedido pedido);
        Task<IEnumerable<Pedido>> ObtenerPedidosConDetallesAsync();
    }


    public interface IDetallePedidoRepository
    {
        Task<int> CrearAsync(DetallePedido detalle);
        Task<IEnumerable<DetallePedido>> ObtenerPorPedidoAsync(int idPedido);
    }


    public interface ILicenciaRepository
    {
        Task<IEnumerable<Licencia>> ObtenerDisponiblesAsync(int idProducto, int cantidad);
        Task<bool> AsignarLicenciasPedidoAsync(int idPedido, int idProducto, int cantidad);
        Task<IEnumerable<Licencia>> ObtenerPorPedidoAsync(int idPedido);
    }


    public interface IUsuarioRepository
    {
        Task<Usuario> ObtenerPorUsuarioAsync(string usuario);
        Task<Usuario> ValidarCredencialesAsync(string usuario, string password);
    }

    public interface IEmailService
    {
        Task<bool> EnviarEmailLicenciasAsync(string destinatario, string nombreCliente, List<string> licencias, string nombreProducto);
    }

}
