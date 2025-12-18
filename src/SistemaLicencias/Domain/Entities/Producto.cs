// Domain/Entities/Producto.cs
namespace Domain.Entities;

public class Producto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; }
    public string Imagen { get; set; }
    public decimal Precio { get; set; }
    public int CantidadLicenciasDisponibles { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
}

public class Cliente
{
    public int IdCliente { get; set; }
    public string Documento { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class Pedido
{
    public int IdPedido { get; set; }
    public int IdCliente { get; set; }
    public DateTime Fecha { get; set; }
    public decimal IGV { get; set; }
    public decimal Total { get; set; }
    public string EstadoPago { get; set; }
    public string EstadoPedido { get; set; }

    // Navegación
    public Cliente Cliente { get; set; }
    public List<DetallePedido> Detalles { get; set; }
}


public class DetallePedido
{
    public int IdDetalle { get; set; }
    public int IdPedido { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    // Navegación
    public Producto Producto { get; set; }
}


public class Licencia
{
    public int IdLicencia { get; set; }
    public int IdProducto { get; set; }
    public string CodigoLicencia { get; set; }
    public int? IdPedido { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public bool Activo { get; set; }
}


public class Usuario
{
    public int IdUsuario { get; set; }
    public string UsuarioNombre { get; set; }
    public string Password { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public bool Activo { get; set; }
}

