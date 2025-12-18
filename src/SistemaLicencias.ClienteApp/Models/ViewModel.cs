using System.ComponentModel.DataAnnotations;

namespace SistemaLicencias.ClienteApp.Models
{

    public class ProductoViewModel
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public decimal Precio { get; set; }
        public int CantidadLicenciasDisponibles { get; set; }
    }

    // Models/CarritoItemViewModel.cs
    public class CarritoItemViewModel
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => Precio * Cantidad;
    }

    // Models/CarritoViewModel.cs
    public class CarritoViewModel
    {
        public List<CarritoItemViewModel> Items { get; set; } = new();
        public decimal Subtotal => Items.Sum(i => i.Subtotal);
        public decimal IGV => Subtotal * 0.18m;
        public decimal Total => Subtotal + IGV;
    }

    // Models/ClienteViewModel.cs
    public class ClienteViewModel
    {
        [Required(ErrorMessage = "El documento es requerido")]
        [StringLength(20, ErrorMessage = "El documento no puede exceder 20 caracteres")]
        [Display(Name = "Documento (DNI/RUC)")]
        public string Documento { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(200, ErrorMessage = "El email no puede exceder 200 caracteres")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }
    }

    // Models/CrearPedidoRequest.cs
    public class CrearPedidoRequest
    {
        public ClienteDto Cliente { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public class ClienteDto
    {
        public string Documento { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }

    // Models/ApiResponse.cs
    public class ApiResponse<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public T Datos { get; set; }
    }

}
