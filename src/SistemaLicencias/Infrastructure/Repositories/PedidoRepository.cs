// Infrastructure/Data/DapperContext.cs
using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;


namespace Infrastructure.Repositories;

// Infrastructure/Repositories/ProductoRepository.cs

public class ProductoRepository : IProductoRepository
{
    private readonly DapperContext _context;

    public ProductoRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
    {
        var query = "SELECT * FROM Productos WHERE Activo = 1 ORDER BY Nombre";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Producto>(query);
    }

    public async Task<Producto> ObtenerPorIdAsync(int id)
    {
        var query = "SELECT * FROM Productos WHERE IdProducto = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Producto>(query, new { Id = id });
    }

    public async Task<int> CrearAsync(Producto producto)
    {
        var query = @"INSERT INTO Productos (Nombre, Imagen, Precio, CantidadLicenciasDisponibles, Activo) 
                      VALUES (@Nombre, @Imagen, @Precio, @CantidadLicenciasDisponibles, @Activo);
                      SELECT CAST(SCOPE_IDENTITY() as int)";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, producto);
    }

    public async Task<bool> ActualizarAsync(Producto producto)
    {
        var query = @"UPDATE Productos 
                      SET Nombre = @Nombre, Imagen = @Imagen, Precio = @Precio, 
                          CantidadLicenciasDisponibles = @CantidadLicenciasDisponibles, Activo = @Activo
                      WHERE IdProducto = @IdProducto";
        using var connection = _context.CreateConnection();
        var rows = await connection.ExecuteAsync(query, producto);
        return rows > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var query = "UPDATE Productos SET Activo = 0 WHERE IdProducto = @Id";
        using var connection = _context.CreateConnection();
        var rows = await connection.ExecuteAsync(query, new { Id = id });
        return rows > 0;
    }
}

// Infrastructure/Repositories/ClienteRepository.cs
public class ClienteRepository : IClienteRepository
{
    private readonly DapperContext _context;

    public ClienteRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Cliente> ObtenerPorDocumentoAsync(string documento)
    {
        var query = "SELECT * FROM Clientes WHERE Documento = @Documento";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Cliente>(query, new { Documento = documento });
    }

    public async Task<Cliente> ObtenerPorIdAsync(int id)
    {
        var query = "SELECT * FROM Clientes WHERE IdCliente = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Cliente>(query, new { Id = id });
    }

    public async Task<int> CrearAsync(Cliente cliente)
    {
        var query = @"INSERT INTO Clientes (Documento, Nombre, Email) 
                      VALUES (@Documento, @Nombre, @Email);
                      SELECT CAST(SCOPE_IDENTITY() as int)";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, cliente);
    }
}

// Infrastructure/Repositories/PedidoRepository.cs
public class PedidoRepository : IPedidoRepository
{
    private readonly DapperContext _context;

    public PedidoRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pedido>> ObtenerTodosAsync()
    {
        var query = "SELECT * FROM Pedidos ORDER BY Fecha DESC";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Pedido>(query);
    }

    public async Task<Pedido> ObtenerPorIdAsync(int id)
    {
        var query = "SELECT * FROM Pedidos WHERE IdPedido = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Pedido>(query, new { Id = id });
    }

    public async Task<int> CrearAsync(Pedido pedido)
    {
        var query = @"INSERT INTO Pedidos (IdCliente, Fecha, IGV, Total, EstadoPago, EstadoPedido) 
                      VALUES (@IdCliente, @Fecha, @IGV, @Total, @EstadoPago, @EstadoPedido);
                      SELECT CAST(SCOPE_IDENTITY() as int)";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, pedido);
    }

    public async Task<bool> ActualizarAsync(Pedido pedido)
    {
        var query = @"UPDATE Pedidos 
                      SET EstadoPago = @EstadoPago, EstadoPedido = @EstadoPedido
                      WHERE IdPedido = @IdPedido";
        using var connection = _context.CreateConnection();
        var rows = await connection.ExecuteAsync(query, pedido);
        return rows > 0;
    }

    public async Task<IEnumerable<Pedido>> ObtenerPedidosConDetallesAsync()
    {
        var query = @"
        SELECT 
            p.IdPedido,
            p.IdCliente,
            p.Fecha,
            p.IGV,
            p.Total,
            p.EstadoPago,
            p.EstadoPedido,
            c.IdCliente,  -- Agregar esto para el splitOn
            c.Documento,
            c.Nombre AS NombreCliente,
            c.Email,
            dp.IdDetalle,  -- Agregar para que DetallePedido tenga su ID
            dp.IdProducto AS DetalleIdProducto,  -- Alias para IdProducto de DetallePedidos
            dp.Cantidad,
            dp.PrecioUnitario,
            dp.Subtotal,
            pr.IdProducto,  -- Esto ahora es único
            pr.Nombre AS NombreProducto,
            pr.Imagen,
            pr.Precio,
            pr.CantidadLicenciasDisponibles,
            pr.FechaCreacion,
            pr.Activo
        FROM Pedidos p
        INNER JOIN Clientes c ON p.IdCliente = c.IdCliente
        INNER JOIN DetallePedidos dp ON p.IdPedido = dp.IdPedido
        INNER JOIN Productos pr ON dp.IdProducto = pr.IdProducto
        ORDER BY p.Fecha DESC";

        using var connection = _context.CreateConnection();

        var pedidosDict = new Dictionary<int, Pedido>();

        await connection.QueryAsync<Pedido, Cliente, DetallePedido, Producto, Pedido>(
            query,
            (pedido, cliente, detalle, producto) =>
            {
                if (!pedidosDict.TryGetValue(pedido.IdPedido, out var pedidoActual))
                {
                    pedidoActual = pedido;
                    pedidoActual.Cliente = cliente;
                    pedidoActual.Detalles = new List<DetallePedido>();
                    pedidosDict.Add(pedido.IdPedido, pedidoActual);
                }

                detalle.Producto = producto;
                pedidoActual.Detalles.Add(detalle);

                return pedidoActual;
            },
            splitOn: "IdCliente,IdDetalle,IdProducto"  // Cambiado aquí
        );

        return pedidosDict.Values;
    }



    public async Task<IEnumerable<Pedido>> ObtenerPedidosConDetallesAsyncOLD()
    {
        var query = @"
            SELECT 
                p.IdPedido,
                p.IdCliente,
                p.Fecha,
                p.IGV,
                p.Total,
                p.EstadoPago,
                p.EstadoPedido,
                c.Documento,
                c.Nombre AS NombreCliente,
                c.Email,
                dp.IdProducto,
                dp.Cantidad,
                dp.PrecioUnitario,
                pr.Nombre AS NombreProducto
            FROM Pedidos p
            INNER JOIN Clientes c ON p.IdCliente = c.IdCliente
            INNER JOIN DetallePedidos dp ON p.IdPedido = dp.IdPedido
            INNER JOIN Productos pr ON dp.IdProducto = pr.IdProducto
            ORDER BY p.Fecha DESC";

        using var connection = _context.CreateConnection();

        var pedidosDict = new Dictionary<int, Pedido>();

        await connection.QueryAsync<Pedido, Cliente, DetallePedido, Producto, Pedido>(
            query,
            (pedido, cliente, detalle, producto) =>
            {
                if (!pedidosDict.TryGetValue(pedido.IdPedido, out var pedidoActual))
                {
                    pedidoActual = pedido;
                    pedidoActual.Cliente = cliente;
                    pedidoActual.Detalles = new List<DetallePedido>();
                    pedidosDict.Add(pedido.IdPedido, pedidoActual);
                }

                detalle.Producto = producto;
                pedidoActual.Detalles.Add(detalle);

                return pedidoActual;
            },
            splitOn: "IdCliente,IdProducto,IdProducto"
        );

        return pedidosDict.Values;
    }

}

// Infrastructure/Repositories/DetallePedidoRepository.cs
public class DetallePedidoRepository : IDetallePedidoRepository
{
    private readonly DapperContext _context;

    public DetallePedidoRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> CrearAsync(DetallePedido detalle)
    {
        var query = @"INSERT INTO DetallePedidos (IdPedido, IdProducto, Cantidad, PrecioUnitario, Subtotal) 
                      VALUES (@IdPedido, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal);
                      SELECT CAST(SCOPE_IDENTITY() as int)";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleAsync<int>(query, detalle);
    }

    public async Task<IEnumerable<DetallePedido>> ObtenerPorPedidoAsync(int idPedido)
    {
        var query = @"SELECT dp.*, p.Nombre AS ProductoNombre 
                      FROM DetallePedidos dp
                      INNER JOIN Productos p ON dp.IdProducto = p.IdProducto
                      WHERE dp.IdPedido = @IdPedido";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<DetallePedido>(query, new { IdPedido = idPedido });
    }
}

// Infrastructure/Repositories/LicenciaRepository.cs
public class LicenciaRepository : ILicenciaRepository
{
    private readonly DapperContext _context;

    public LicenciaRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Licencia>> ObtenerDisponiblesAsync(int idProducto, int cantidad)
    {
        var query = @"SELECT TOP (@Cantidad) * FROM Licencias 
                      WHERE IdProducto = @IdProducto AND IdPedido IS NULL AND Activo = 1
                      ORDER BY IdLicencia";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Licencia>(query, new { IdProducto = idProducto, Cantidad = cantidad });
    }

    public async Task<bool> AsignarLicenciasPedidoAsync(int idPedido, int idProducto, int cantidad)
    {
        using var connection = _context.CreateConnection();
        var rows = await connection.ExecuteAsync("sp_AsignarLicenciasPedido",
            new { IdPedido = idPedido, IdProducto = idProducto, Cantidad = cantidad },
            commandType: System.Data.CommandType.StoredProcedure);
        return rows > 0;
    }

    public async Task<IEnumerable<Licencia>> ObtenerPorPedidoAsync(int idPedido)
    {
        var query = "SELECT * FROM Licencias WHERE IdPedido = @IdPedido";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Licencia>(query, new { IdPedido = idPedido });
    }
}

// Infrastructure/Repositories/UsuarioRepository.cs
public class UsuarioRepository : IUsuarioRepository
{
    private readonly DapperContext _context;

    public UsuarioRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Usuario> ObtenerPorUsuarioAsync(string usuario)
    {
        var query = "SELECT * FROM Usuarios WHERE Usuario = @Usuario AND Activo = 1";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Usuario>(query, new { Usuario = usuario });
    }

    public async Task<Usuario> ValidarCredencialesAsync(string usuario, string password)
    {
        var query = "SELECT * FROM Usuarios WHERE Usuario = @Usuario AND Password = @Password AND Activo = 1";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Usuario>(query, new { Usuario = usuario, Password = password });
    }
}

// Infrastructure/Services/EmailService.cs

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> EnviarEmailLicenciasAsync(string destinatario, string nombreCliente, List<string> licencias, string nombreProducto)
    {
        try
        {
            _logger.LogInformation("Enviando email de licencias a {Email}", destinatario);

            // Configurar aquí el servicio SMTP real
            // Por ahora simulamos el envío
            var mensaje = $@"
                <html>
                <body>
                    <h2>¡Gracias por tu compra, {nombreCliente}!</h2>
                    <p>Tu pedido ha sido procesado exitosamente.</p>
                    <h3>Producto: {nombreProducto}</h3>
                    <h4>Licencias adquiridas:</h4>
                    <ul>
                    {string.Join("", licencias.Select(l => $"<li>{l}</li>"))}
                    </ul>
                    <p>Guarda estas licencias en un lugar seguro.</p>
                </body>
                </html>
            ";

            _logger.LogInformation("Email enviado exitosamente a {Email}", destinatario);
            _logger.LogDebug("Contenido: {Mensaje}", mensaje);

            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email a {Email}", destinatario);
            return false;
        }
    }
}