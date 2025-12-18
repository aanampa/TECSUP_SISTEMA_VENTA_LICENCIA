using Newtonsoft.Json;
using System.Text;

// SistemaLicencias.ClienteApp/Models/NiubizModels.cs
namespace SistemaLicencias.ClienteApp.Models;

public class 
    GenerarTokenNiubizRequest
{
    public int IdPedido { get; set; }
    public decimal Monto { get; set; }
    public string Email { get; set; }
    public string Documento { get; set; }
    public string ClientIp { get; set; }
}

public class GenerarTokenNiubizResponse
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; }
    public string SessionToken { get; set; }
    public string PurchaseNumber { get; set; }
    public decimal Amount { get; set; }
    public long ExpirationTime { get; set; }
}   

public class ConfirmarPagoNiubizRequest
{
    public int IdPedido { get; set; }
    public string TransactionToken { get; set; }
    public double Amount { get; set; }
}

public class ConfirmarPagoNiubizResponse
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; }
    public string TransactionId { get; set; }
    public string AuthorizationCode { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
}


