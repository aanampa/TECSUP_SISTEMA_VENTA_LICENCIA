// Application/Services/NiubizService.cs
using Application.DTOs.Niubiz;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Reflection.Metadata;
using System.Text;

namespace Application.Services;

public class NiubizService : INiubizService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NiubizService> _logger;
    private readonly NiubizConfigDto _config;

    public NiubizService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<NiubizService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _config = new NiubizConfigDto
        {
            MerchantId = configuration["Niubiz:MerchantId"],
            Username = configuration["Niubiz:Username"],
            Password = configuration["Niubiz:Password"],
            BaseUrl = configuration["Niubiz:BaseUrl"],
            Currency = configuration["Niubiz:Currency"] ?? "PEN",
            AccessUrl = configuration["Niubiz:AccessUrl"] ?? "",
            SesionUrl = configuration["Niubiz:SesionUrl"] ?? "",
            AuthorizationUrl = configuration["Niubiz:AuthorizationUrl"] ?? ""
        };

        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
    }

    public async Task<GenerarTokenResponse> GenerarTokenSesionAsync(GenerarTokenRequest request)
    {
        try
        {

            

            _logger.LogInformation("Generando token de sesión para pedido {IdPedido}", request.IdPedido);

            // Paso 1: Obtener Access Token
            var accessToken = await ObtenerAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return new GenerarTokenResponse
                {
                    Exito = false,
                    Mensaje = "No se pudo obtener el access token"
                };
            }

            //var remoteIp = HttpContext.Connection.RemoteIpAddress.ToString();

            var remoteIp = "127.0.0.1"; 
            string sss = await GetSessionToken(accessToken, 120, "antonio.anampa@gmail.com","09822819" , remoteIp);

            // Paso 2: Generar Session Token
            var purchaseNumber = $"PED{request.IdPedido:D8}";

            var tokenRequest = new TokenizacionRequest
            {
                Channel = new Channel { Type = "web" },
                CaptureType = "manual",
                Countable = true,
                Order = new Order
                {
                    tokenId = Guid.NewGuid().ToString(),
                    purchaseNumber = purchaseNumber,
                    amount = request.Monto,
                    currency = _config.Currency
                }
            };

            var json = JsonConvert.SerializeObject(tokenRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);

            //string merchantId = _config.MerchantId;
            //string url = _config.SesionUrl;
            //url = url.Replace("{merchantId}", merchantId);
            ////https://apitestenv.vnforapps.com/api.ecommerce/v2/ecommerce/token/session/{merchantId}


            var response = await _httpClient.PostAsync(
                $"/api.security/v1/security",
                content
            );

            //var response = await _httpClient.PostAsync(
            //    url,
            //    content
            //);


            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Respuesta Niubiz: {Response}", responseContent);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenizacionResponse>(responseContent);

                return new GenerarTokenResponse
                {
                    Exito = true,
                    Mensaje = "Token generado exitosamente",
                    SessionToken = tokenResponse.SessionKey,
                    PurchaseNumber = purchaseNumber,
                    Amount = request.Monto,
                    ExpirationTime = tokenResponse.ExpirationTime
                };
            }

            _logger.LogError("Error al generar token. Status: {StatusCode}, Response: {Response}",
                response.StatusCode, responseContent);

            return new GenerarTokenResponse
            {
                Exito = false,
                Mensaje = $"Error al generar token: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al generar token de sesión");
            return new GenerarTokenResponse
            {
                Exito = false,
                Mensaje = "Error interno al generar token"
            };
        }
    }

    public async Task<GenerarTokenResponse> GenerarTokenSesionAsync2(GenerarTokenRequest request)
    {
        _logger.LogInformation($"Ejecutando GenerarTokenSesionAsync2()");
        try
        {

            _logger.LogInformation("Generando token de sesión para pedido {IdPedido}", request.IdPedido);

            // Paso 1: Obtener Access Token
            var accessToken = await ObtenerAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return new GenerarTokenResponse
                {
                    Exito = false,
                    Mensaje = "No se pudo obtener el access token"
                };
            }

            //---------
            string merchantId = _config.MerchantId;
            string url = _config.SesionUrl.Replace("{merchantId}", merchantId);

            RootPagoWeb requestModel = new();

            requestModel.channel = "web";
            requestModel.amount = request.Monto;
            requestModel.antifraud = new();
            requestModel.antifraud.clientIp = request.ClientIp;
            requestModel.antifraud.merchantDefineData = new();
            requestModel.antifraud.merchantDefineData.MDD4 = request.Email;
            requestModel.antifraud.merchantDefineData.MDD21 = 0;
            requestModel.antifraud.merchantDefineData.MDD32 = request.Documento;
            requestModel.antifraud.merchantDefineData.MDD75 = "Invitado"; //"Registrado"
            requestModel.antifraud.merchantDefineData.MDD77 = 1; //Desde cuando se registo el cliente

            var json = JsonConvert.SerializeObject(requestModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug($"- json: {json}");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);

            var response = await _httpClient.PostAsync(
                url,
                content
            );

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogDebug("- Respuesta Niubiz: {Response}", responseContent);

                var tokenResponse = JsonConvert.DeserializeObject<RootSessionToken>(responseContent);
                var sessionToken = tokenResponse?.sessionKey ?? "";
                var purchaseNumber = $"{request.IdPedido:D8}";

                return new GenerarTokenResponse
                {
                    Exito = true,
                    Mensaje = "Token generado exitosamente",
                    SessionToken = sessionToken,
                    PurchaseNumber = purchaseNumber,
                    Amount = request.Monto,
                    ExpirationTime = tokenResponse.expirationTime
                };

            }

            return new GenerarTokenResponse
            {
                Exito = false,
                Mensaje = $"Error al generar token: {response.StatusCode}"
            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al generar token de sesión");
            return new GenerarTokenResponse
            {
                Exito = false,
                Mensaje = "Error interno al generar token"
            };
        }
    }

    private async Task<string> GetSessionToken(string accessToken, double amount, string email, string document, string clientIp)
    {
        _logger.LogInformation("PagoWebController.GetSessionToken()");

        //Token de acceso
        string merchantId = _config.MerchantId;
        string url2 = _config.SesionUrl.Replace("{merchantId}", merchantId);
        string tokenSesion = "";

        _logger.LogInformation("- url: " + url2);
        _logger.LogInformation($"- tokenAcceso: {accessToken}");
        _logger.LogInformation($"- amount: {amount}");
        _logger.LogInformation($"- email: {email}");
        _logger.LogInformation($"- document: {document}");
        _logger.LogInformation($"- clientIp: {clientIp}");

        RootPagoWeb requestModel = new();

        requestModel.channel = "web";
        requestModel.amount = amount;
        requestModel.antifraud = new();
        requestModel.antifraud.clientIp = clientIp;
        requestModel.antifraud.merchantDefineData = new();
        requestModel.antifraud.merchantDefineData.MDD4 = email;
        requestModel.antifraud.merchantDefineData.MDD21 = 0;
        requestModel.antifraud.merchantDefineData.MDD32 = document;
        requestModel.antifraud.merchantDefineData.MDD75 = "Invitado"; //"Registrado"
        requestModel.antifraud.merchantDefineData.MDD77 = 1; //Desde cuando se registo el cliente

        var json = JsonConvert.SerializeObject(requestModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);

        var response = await _httpClient.PostAsync(
            url2,
            content
        );

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogDebug("Respuesta Niubiz: {Response}", responseContent);

            var tokenResponse = JsonConvert.DeserializeObject<RootSessionToken>(responseContent);

            tokenSesion = tokenResponse?.sessionKey ?? "";

        }
        else
        {
            tokenSesion = "";
        }



        return tokenSesion;

    }

    public async Task<ConfirmarPagoResponse> AutorizarTransaccionAsync(ConfirmarPagoRequest request)
    {
        try
        {
            _logger.LogInformation("Autorizando transacción para pedido {IdPedido}", request.IdPedido);

            // Obtener Access Token
            var accessToken = await ObtenerAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                return new ConfirmarPagoResponse
                {
                    Exito = false,
                    Mensaje = "No se pudo obtener el access token"
                };
            }

            // Autorizar transacción
            var purchaseNumber = $"PED{request.IdPedido:D8}";

            var autorizacionRequest = new AutorizacionRequest
            {
                Channel = new Channel { Type = "web" },
                CaptureType = "manual",
                Countable = true,
                Order = new OrderAutorizacion
                {
                    TokenId = request.TransactionToken,
                    PurchaseNumber = purchaseNumber,
                    Amount = 0, // Se obtiene del token
                    Currency = _config.Currency
                }
            };

            var json = JsonConvert.SerializeObject(autorizacionRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);

            var response = await _httpClient.PostAsync(
                $"/api.authorization/v3/authorization/ecommerce/{_config.MerchantId}",
                content
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Respuesta autorización: {Response}", responseContent);

            if (response.IsSuccessStatusCode)
            {
                var autorizacionResponse = JsonConvert.DeserializeObject<AutorizacionResponse>(responseContent);

                return new ConfirmarPagoResponse
                {
                    Exito = autorizacionResponse.Order.Status == "Authorized",
                    Mensaje = autorizacionResponse.DataMap?.ActionDescription ?? "Transacción procesada",
                    TransactionId = autorizacionResponse.Order.TransactionId,
                    AuthorizationCode = autorizacionResponse.Order.AuthorizationCode,
                    Status = autorizacionResponse.Order.Status,
                    Amount = autorizacionResponse.Order.Amount
                };
            }

            _logger.LogError("Error al autorizar. Status: {StatusCode}, Response: {Response}",
                response.StatusCode, responseContent);

            return new ConfirmarPagoResponse
            {
                Exito = false,
                Mensaje = $"Error al autorizar: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al autorizar transacción");
            return new ConfirmarPagoResponse
            {
                Exito = false,
                Mensaje = "Error interno al autorizar transacción"
            };
        }
    }

    public async Task<ConfirmarPagoResponse> AutorizarTransaccionAsync2(ConfirmarPagoRequest request)
    {
        _logger.LogInformation($"Ejecutando AutorizarTransaccionAsync2()");

        try
        {
            _logger.LogInformation("Autorizando transacción para pedido {IdPedido}", request.IdPedido);

            // Obtener Access Token
            var accessToken = await ObtenerAccessTokenAsync2();
            if (string.IsNullOrEmpty(accessToken))
            {
                return new ConfirmarPagoResponse
                {
                    Exito = false,
                    Mensaje = "No se pudo obtener el access token"
                };
            }

            //request.Amount = 82.59;

            // Autorizar transacción
            var purchaseNumber = $"{request.IdPedido:D8}";

            AutorizacionPagoWeb autorizacionRequest = new();
            autorizacionRequest.channel = "web";
            autorizacionRequest.captureType = "manual";
            autorizacionRequest.countable = true;
            autorizacionRequest.order = new();
            autorizacionRequest.order.tokenId = request.TransactionToken;
            autorizacionRequest.order.purchaseNumber = purchaseNumber;
            autorizacionRequest.order.amount = request.Amount;
            autorizacionRequest.order.currency = _config.Currency;

            //var autorizacionRequest = new AutorizacionRequest
            //{
            //    Channel = new Channel { Type = "web" },
            //    CaptureType = "manual",
            //    Countable = true,
            //    Order = new OrderAutorizacion
            //    {
            //        TokenId = request.TransactionToken,
            //        PurchaseNumber = purchaseNumber,
            //        Amount = request.Amount, // Se obtiene del token
            //        Currency = _config.Currency
            //    }
            //};

            var url = _config.AuthorizationUrl.Replace("{merchantId}", _config.MerchantId);
            var json = JsonConvert.SerializeObject(autorizacionRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("- url: {url}", url);
            _logger.LogInformation("- json: {json}", json);
            _logger.LogInformation("- json: {content}", content);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);

            var response = await _httpClient.PostAsync(
                url,
                content
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Respuesta autorización: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                var autorizacionResponse = JsonConvert.DeserializeObject<AutorizationResponse>(responseContent);

                if (autorizacionResponse == null)
                {
                    return new ConfirmarPagoResponse
                    {
                        Exito = false,
                        Mensaje = "No se pudo autorizar"
                    };
                }

                return new ConfirmarPagoResponse
                {
                    Exito = true, // autorizacionResponse.order.status == "Authorized",
                    Mensaje = autorizacionResponse.dataMap?.ACTION_DESCRIPTION ?? "Transacción procesada",
                    TransactionId = autorizacionResponse.order.transactionId,
                    AuthorizationCode = autorizacionResponse.order.authorizationCode,
                    Status = "Authorized",
                    Amount = autorizacionResponse.order.amount
                };
            }

            _logger.LogError("Error al autorizar. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, responseContent);

            return new ConfirmarPagoResponse
            {
                Exito = false,
                Mensaje = $"Error al autorizar: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al autorizar transacción");
            return new ConfirmarPagoResponse
            {
                Exito = false,
                Mensaje = "Error interno al autorizar transacción"
            };
        }
    }
    public async Task<string> ObtenerAccessTokenAsync()
    {
        try
        {
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_config.Username}:{_config.Password}")
            );


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");

            //var response = await _httpClient.GetAsync(
            //    $"/api.security/v1/security/merchants/{_config.MerchantId}"
            //);

            var response = await _httpClient.GetAsync(
                _config.AccessUrl
            );

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Access token obtenido exitosamente");
                return content.Trim('"'); // Niubiz retorna el token entre comillas
            }

            _logger.LogError("Error al obtener access token. Status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al obtener access token");
            return null;
        }
    }

    public async Task<string> ObtenerAccessTokenAsync2()
    {
        try
        {
            string userName = _config.Username;
            string password = _config.Password;
            string url = _config.AccessUrl;

            var options = new RestClientOptions(url)
            {
                Authenticator = new HttpBasicAuthenticator(userName, password)
            };

            using var client = new RestClient(options);
            var request = new RestRequest();


            var response = await client.GetAsync(request);

            if (response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode} - {response.ErrorMessage}");
            }

     
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al obtener access token");
            return null;
        }
    }



}