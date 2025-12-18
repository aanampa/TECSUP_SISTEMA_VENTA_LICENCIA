// Models/ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SistemaLicencias.AdminApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RecordarMe { get; set; }
    }


    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200)]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; }

        [Display(Name = "Imagen (URL)")]
        [StringLength(500)]
        public string Imagen { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio (S/)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0, 999999, ErrorMessage = "La cantidad debe ser mayor o igual a 0")]
        [Display(Name = "Licencias Disponibles")]
        public int CantidadLicenciasDisponibles { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime? FechaCreacion { get; set; }
    }

    public class PedidoViewModel
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string DocumentoCliente { get; set; }
        public string EmailCliente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal IGV { get; set; }
        public decimal Total { get; set; }
        public string EstadoPago { get; set; }
        public string EstadoPedido { get; set; }
        public List<DetallePedidoViewModel> Detalles { get; set; } = new();
    }


    public class DetallePedidoViewModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal => Precio * Cantidad;
    }

    public class ClienteViewModel
    {
        public int IdCliente { get; set; }
        public string Documento { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int TotalPedidos { get; set; }
        public decimal TotalCompras { get; set; }
    }

    public class LicenciaViewModel
    {
        public int IdLicencia { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoLicencia { get; set; }
        public int? IdPedido { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public bool Activo { get; set; }
        public string Estado => IdPedido.HasValue ? "Asignada" : "Disponible";
    }


    public class DashboardViewModel
    {
        public EstadisticasViewModel Estadisticas { get; set; }
        public List<PedidoViewModel> PedidosRecientes { get; set; }
        public List<ProductoMasVendidoViewModel> ProductosMasVendidos { get; set; }
    }


    public class EstadisticasViewModel
    {
        public decimal VentasHoy { get; set; }
        public decimal VentasMes { get; set; }
        public int PedidosHoy { get; set; }
        public int PedidosMes { get; set; }
        public int TotalClientes { get; set; }
        public int LicenciasDisponibles { get; set; }
    }

    public class ProductoMasVendidoViewModel
    {
        public string NombreProducto { get; set; }
        public int CantidadVendida { get; set; }
        public decimal TotalVentas { get; set; }
    }


}
