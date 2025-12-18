// Application/DTOs/ProductoDto.cs
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class ProductoDto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; }
    public string Imagen { get; set; }
    public decimal Precio { get; set; }
    public int CantidadLicenciasDisponibles { get; set; }
}

// Application/DTOs/ClienteDto.cs
public class ClienteDto
{
    [Required(ErrorMessage = "El documento es requerido")]
    [StringLength(20, ErrorMessage = "El documento no puede exceder 20 caracteres")]
    public string Documento { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(200, ErrorMessage = "El email no puede exceder 200 caracteres")]
    public string Email { get; set; }
}

// Application/DTOs/CrearPedidoDto.cs
public class CrearPedidoDto
{
    [Required(ErrorMessage = "Los datos del cliente son requeridos")]
    public ClienteDto Cliente { get; set; }

    [Required(ErrorMessage = "El ID del producto es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser mayor a 0")]
    public int IdProducto { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100")]
    public int Cantidad { get; set; }
}

// Application/DTOs/PedidoDto.cs
public class PedidoDto
{
    public int IdPedido { get; set; }
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; }
    public string EmailCliente { get; set; }
    public DateTime Fecha { get; set; }
    public decimal IGV { get; set; }
    public decimal Total { get; set; }
    public string EstadoPago { get; set; }
    public string EstadoPedido { get; set; }
    public List<DetallePedidoDto> Detalles { get; set; }
}

// Application/DTOs/DetallePedidoDto.cs
public class DetallePedidoDto
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
}

// Application/DTOs/ResultadoDto.cs
public class ResultadoDto<T>
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; }
    public T Datos { get; set; }

    public static ResultadoDto<T> ExitoResultado(T datos, string mensaje = "Operación exitosa")
    {
        return new ResultadoDto<T>
        {
            Exito = true,
            Mensaje = mensaje,
            Datos = datos
        };
    }

    public static ResultadoDto<T> ErrorResultado(string mensaje)
    {
        return new ResultadoDto<T>
        {
            Exito = false,
            Mensaje = mensaje,
            Datos = default
        };
    }
}

public class ActualizarEstadoPedidoDto
{
    [Required(ErrorMessage = "El estado de pago es requerido")]
    [StringLength(50, ErrorMessage = "El estado de pago no puede exceder 50 caracteres")]
    public string EstadoPago { get; set; }

    [Required(ErrorMessage = "El estado del pedido es requerido")]
    [StringLength(50, ErrorMessage = "El estado del pedido no puede exceder 50 caracteres")]
    public string EstadoPedido { get; set; }
}