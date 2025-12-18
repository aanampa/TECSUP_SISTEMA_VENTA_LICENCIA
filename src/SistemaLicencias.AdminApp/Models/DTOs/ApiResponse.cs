namespace SistemaLicencias.AdminApp.Models.DTOs
{


    public class ApiResponse<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public T Datos { get; set; }
    }

    // Models/DTOs/ActualizarEstadoPedidoDto.cs
    public class ActualizarEstadoPedidoDto
    {
        public string EstadoPago { get; set; }
        public string EstadoPedido { get; set; }
    }
}
