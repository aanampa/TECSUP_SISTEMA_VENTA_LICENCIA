using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Niubiz
{

    // Application/DTOs/Niubiz/NiubizConfigDto.cs
    public class NiubizConfigDto
    {
        public string MerchantId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }
        public string Currency { get; set; } = "PEN";
        public string AccessUrl { get; set; } = "";
        public string SesionUrl { get; set; } = "";
        public string AuthorizationUrl { get; set; } = "";
        

    }

    // Application/DTOs/Niubiz/TokenizacionRequest.cs

    public class TokenizacionRequest
    {
        public Channel Channel { get; set; }
        public string CaptureType { get; set; } = "manual";
        public bool Countable { get; set; } = true;
        public Order Order { get; set; }
    }

    public class Channel
    {
        public string Type { get; set; } = "web";
    }

    public class Order
    {
        public string tokenId { get; set; }
        public string purchaseNumber { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
    }

    // Application/DTOs/Niubiz/TokenizacionResponse.cs
    public class TokenizacionResponse
    {
        public string SessionKey { get; set; }
        public int ExpirationTime { get; set; }
        public DataMap2 DataMap { get; set; }
    }

    public class DataMap2
    {
        public string CardType { get; set; }
    }

    // Application/DTOs/Niubiz/AutorizacionRequest.cs
    public class AutorizacionRequest
    {
        public Channel Channel { get; set; }
        public string CaptureType { get; set; } = "manual";
        public bool Countable { get; set; } = true;
        public OrderAutorizacion Order { get; set; }
    }

    public class OrderAutorizacion
    {
        public string TokenId { get; set; }
        public string PurchaseNumber { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    // Application/DTOs/Niubiz/AutorizacionResponse.cs
    public class AutorizacionResponse
    {
        public Header Header { get; set; }
        public OrderResponse2 Order { get; set; }
        public DataMapResponse DataMap { get; set; }
    }

    public class Header2
    {
        public string EcoreTransactionUUID { get; set; }
        public string EcoreTransactionDate { get; set; }
        public int Millis { get; set; }
    }

    public class OrderResponse2
    {
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public string PurchaseNumber { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string AuthorizationCode { get; set; }
        public string ActionCode { get; set; }
        public string TraceNumber { get; set; }
        public int TransactionDate { get; set; }
    }

    public class DataMapResponse
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string ActionDescription { get; set; }
        public string Brand { get; set; }
        public string IIN { get; set; }
        public string MERCHANT_ID { get; set; }
        public string TimeStamp { get; set; }
    }

    // Application/DTOs/Niubiz/GenerarTokenRequest.cs
    public class GenerarTokenRequest
    {
        public int IdPedido { get; set; }
        public double Monto { get; set; }
        public string Email { get; set; }
        public string Documento { get; set; }
        public string ClientIp { get; set; }
        
    }

    // Application/DTOs/Niubiz/GenerarTokenResponse.cs
    public class GenerarTokenResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string SessionToken { get; set; }
        public string PurchaseNumber { get; set; }
        public double Amount { get; set; }
        public long ExpirationTime { get; set; }
    }

    // Application/DTOs/Niubiz/ConfirmarPagoRequest.cs
    public class ConfirmarPagoRequest
    {
        public int IdPedido { get; set; }
        public string TransactionToken { get; set; }
        public double Amount { get; set; }
    }

    // Application/DTOs/Niubiz/ConfirmarPagoResponse.cs
    public class ConfirmarPagoResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string TransactionId { get; set; }
        public string AuthorizationCode { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
    }

    public class ApiResponse
    {
        public string Data { get; set; }
        public bool Status { get; set; }
    }
}
