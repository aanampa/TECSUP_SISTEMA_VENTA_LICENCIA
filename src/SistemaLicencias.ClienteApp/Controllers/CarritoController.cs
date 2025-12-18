using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaLicencias.ClienteApp.Models;
using SistemaLicencias.ClienteApp.Services;

namespace SistemaLicencias.ClienteApp.Controllers
{

    public class CarritoController : Controller
    {
        private readonly ILicenciasApiService _apiService;
        private readonly IMailingApiService _apiMailingService;
        private readonly ILogger<CarritoController> _logger;
        private readonly IConfiguration _configuration;
        private const string CarritoSessionKey = "Carrito";
        private const string ClienteSessionKey = "Cliente";
        private const string PedidoTempSessionKey = "PedidoTemp";

        public CarritoController(
            ILicenciasApiService apiService,
            IMailingApiService apiMailingService,
            ILogger<CarritoController> logger,
            IConfiguration configuration)
        {
            _apiService = apiService;
            _logger = logger;
            _configuration = configuration;
            _apiMailingService = apiMailingService;
        }

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarProducto(int idProducto, int cantidad = 1)
        {
            var producto = await _apiService.ObtenerProductoPorIdAsync(idProducto);

            if (producto == null)
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction("Index", "Home");
            }

            if (cantidad > producto.CantidadLicenciasDisponibles)
            {
                TempData["Error"] = "No hay suficientes licencias disponibles";
                return RedirectToAction("Index", "Home");
            }

            var carrito = ObtenerCarrito();
            var itemExistente = carrito.Items.FirstOrDefault(i => i.IdProducto == idProducto);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Items.Add(new CarritoItemViewModel
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });
            }

            GuardarCarrito(carrito);
            TempData["Mensaje"] = "Producto agregado al carrito";

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ActualizarCantidad(int idProducto, int cantidad)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.Items.FirstOrDefault(i => i.IdProducto == idProducto);

            if (item != null)
            {
                if (cantidad > 0)
                {
                    item.Cantidad = cantidad;
                }
                else
                {
                    carrito.Items.Remove(item);
                }
                GuardarCarrito(carrito);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EliminarProducto(int idProducto)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.Items.FirstOrDefault(i => i.IdProducto == idProducto);

            if (item != null)
            {
                carrito.Items.Remove(item);
                GuardarCarrito(carrito);
                TempData["Mensaje"] = "Producto eliminado del carrito";
            }

            return RedirectToAction("Index");
        }

        public IActionResult DatosCliente()
        {
            var carrito = ObtenerCarrito();

            if (!carrito.Items.Any())
            {
                TempData["Error"] = "El carrito está vacío";
                return RedirectToAction("Index");
            }

            return View(new ClienteViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarPedidoConNiubiz(ClienteViewModel clienteViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("DatosCliente", clienteViewModel);
            }

            var carrito = ObtenerCarrito();

            if (!carrito.Items.Any())
            {
                TempData["Error"] = "El carrito está vacío";
                return RedirectToAction("Index");
            }

            try
            {
                // Crear pedido temporal (sin pagar aún)
                var pedidosCreados = new List<int>();

                foreach (var item in carrito.Items)
                {
                    var pedido = new CrearPedidoRequest
                    {
                        Cliente = new ClienteDto
                        {
                            Documento = clienteViewModel.Documento,
                            Nombre = clienteViewModel.Nombre,
                            Email = clienteViewModel.Email
                        },
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad
                    };

                    var resultado = await _apiService.CrearPedidoAsync(pedido);

                    if (resultado.Exito)
                    {
                        pedidosCreados.Add(resultado.Datos);
                    }
                }

                if (pedidosCreados.Any())
                {
                    // Guardar datos del cliente en sesión
                    GuardarCliente(clienteViewModel);

                    // Guardar IDs de pedidos en sesión
                    HttpContext.Session.SetString(PedidoTempSessionKey,
                        JsonConvert.SerializeObject(pedidosCreados));

                    // Generar token de Niubiz para el primer pedido
                    var totalAmount = Math.Round(carrito.Total, 2); // Redondear a 2 decimales
                    var primerPedido = pedidosCreados.First();

                    var remoteIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                    if (remoteIp == "::1") { remoteIp = "127.0.0.1"; }

                    var tokenRequest = new GenerarTokenNiubizRequest
                    {
                        IdPedido = primerPedido,
                        Monto = totalAmount,
                        Email = clienteViewModel.Email,
                        Documento = clienteViewModel.Documento,
                        ClientIp = remoteIp
                    };

                    var tokenResponse = await _apiService.GenerarTokenNiubizAsync(tokenRequest);

                    if (tokenResponse.Exito)
                    {
                        // Redirigir a página de pago con Niubiz
                        ViewBag.SessionToken = tokenResponse.SessionToken;
                        ViewBag.PurchaseNumber = tokenResponse.PurchaseNumber;
                        ViewBag.Amount = tokenResponse.Amount;
                        ViewBag.MerchantId = _configuration["Niubiz:MerchantId"];
                        ViewBag.NiubizScriptUrl = _configuration["Niubiz:ScriptUrl"];
                        ViewBag.Carrito = carrito;

                        return View("PagoNiubiz", clienteViewModel);
                    }
                    else
                    {
                        TempData["Error"] = "Error al iniciar el proceso de pago: " + tokenResponse.Mensaje;
                        return View("DatosCliente", clienteViewModel);
                    }
                }
                else
                {
                    TempData["Error"] = "No se pudo crear ningún pedido";
                    return View("DatosCliente", clienteViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar pedidos");
                TempData["Error"] = "Error al procesar la compra. Intenta nuevamente.";
                return View("DatosCliente", clienteViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPagoNiubiz([FromBody] ConfirmarPagoNiubizRequest request)
        {
            try
            {
                var pedidosJson = HttpContext.Session.GetString(PedidoTempSessionKey);
                if (string.IsNullOrEmpty(pedidosJson))
                {
                    return Json(new { exito = false, mensaje = "No se encontró información del pedido" });
                }

                var carrito = ObtenerCarrito();

                var pedidosIds = JsonConvert.DeserializeObject<List<int>>(pedidosJson);
                var primerPedido = pedidosIds.First();

                // Confirmar pago con Niubiz
                var confirmarRequest = new ConfirmarPagoNiubizRequest
                {
                    IdPedido = primerPedido,
                    TransactionToken = request.TransactionToken
                };

                var resultado = await _apiService.ConfirmarPagoNiubizAsync(confirmarRequest);

                if (resultado.Exito)
                {
                    // Limpiar carrito y sesión
                    LimpiarCarrito2();
                    HttpContext.Session.Remove(PedidoTempSessionKey);
                    HttpContext.Session.Remove(ClienteSessionKey);

                    _logger.LogInformation("Pago confirmado: {TransactionId}", resultado.TransactionId);

                    return Json(new
                    {
                        exito = true,
                        mensaje = "Pago procesado exitosamente",
                        transactionId = resultado.TransactionId
                    });
                }
                else
                {
                    _logger.LogWarning("Error al confirmar pago: {Mensaje}", resultado.Mensaje);
                    return Json(new { exito = false, mensaje = resultado.Mensaje });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar pago Niubiz");
                return Json(new { exito = false, mensaje = "Error al procesar la confirmación del pago" });
            }
        }

        [HttpPost]
        [HttpGet] // Agregar GET también porque Niubiz puede redirigir con parámetros en URL
        public async Task<IActionResult> ProcesarPagoNiubiz(string transactionToken)
        {
            _logger.LogInformation("ProcesarPagoNiubiz llamado. Method: {Method}, TransactionToken: {Token}",
                Request.Method, transactionToken);

            // Log de todos los parámetros recibidos
            _logger.LogInformation("Query params: {Query}", Request.Query.ToString());
            _logger.LogInformation("Form params: {Form}", Request.HasFormContentType ? Request.Form.ToString() : "N/A");

            var carrito = ObtenerCarrito();
            var cliente = ObtenerCliente();

            // Niubiz redirige aquí después de completar el pago
            // El transactionToken puede venir en Query o Form
            if (string.IsNullOrEmpty(transactionToken))
            {
                // Intentar obtener de otros lugares
                transactionToken = Request.Query["transactionToken"].FirstOrDefault()
                                ?? Request.Form["transactionToken"].FirstOrDefault();

                _logger.LogInformation("TransactionToken obtenido de parámetros: {Token}", transactionToken);
            }

            if (string.IsNullOrEmpty(transactionToken))
            {
                _logger.LogError("No se recibió transactionToken");
                TempData["Error"] = "No se recibió el token de transacción";
                return RedirectToAction("Index");
            }

            try
            {
                var pedidosJson = HttpContext.Session.GetString(PedidoTempSessionKey);
                if (string.IsNullOrEmpty(pedidosJson))
                {
                    _logger.LogWarning("No se encontró información del pedido en sesión");
                    TempData["Error"] = "No se encontró información del pedido";
                    return RedirectToAction("Index");
                }

                var pedidosIds = JsonConvert.DeserializeObject<List<int>>(pedidosJson);
                var primerPedido = pedidosIds.First();

                _logger.LogInformation("Confirmando pago para pedido {IdPedido}", primerPedido);

                // Confirmar pago con Niubiz
                var confirmarRequest = new ConfirmarPagoNiubizRequest
                {
                    IdPedido = primerPedido,
                    TransactionToken = transactionToken, 
                    Amount = Math.Round( Convert.ToDouble( carrito.Total), 2 )
                };

                var resultado = await _apiService.ConfirmarPagoNiubizAsync(confirmarRequest);

                if (resultado.Exito)
                {
                    // Limpiar carrito y sesión
                    LimpiarCarrito2();
                    HttpContext.Session.Remove(PedidoTempSessionKey);
                    HttpContext.Session.Remove(ClienteSessionKey);

                    try
                    {
                        string asunto = "Licencias enviadas";
                        string contenido = "Estimado " + cliente.Nombre + ",<br><br>A travez del siguiente Link podra descargar La licencias del producto adquirido.<br><br><a href=\"#\">[Desgargar Licencia]</a><br/><br/>Gracias por usar nuestros servicios.<br><br>Atentamente,<br>Sistema de Licencias";
                        await _apiMailingService.EnviarEmailAsync(cliente.Email, cliente.Nombre, asunto, contenido);

                    }
                    catch (Exception exmail)
                    {
                        _logger.LogError($"Error al enviar correo: {exmail}");
                    }

                    _logger.LogInformation("Pago confirmado exitosamente: {TransactionId}", resultado.TransactionId);

                    // Redirigir a página de confirmación
                    TempData["Mensaje"] = "¡Pago procesado exitosamente! Las licencias han sido enviadas a tu correo.";
                    return RedirectToAction("Confirmacion");
                }
                else
                {
                    _logger.LogWarning("Error al confirmar pago: {Mensaje}", resultado.Mensaje);
                    TempData["Error"] = "Error al procesar el pago: " + resultado.Mensaje;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar pago Niubiz");
                TempData["Error"] = "Error al procesar el pago. Por favor contacta con soporte.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Confirmacion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LimpiarCarrito()
        {
            HttpContext.Session.Remove(CarritoSessionKey);
            TempData["Mensaje"] = "Carrito vaciado";
            return RedirectToAction("Index");
        }

        // ==================== MÉTODOS PRIVADOS ====================

        private CarritoViewModel ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString(CarritoSessionKey);

            if (string.IsNullOrEmpty(carritoJson))
            {
                return new CarritoViewModel();
            }

            return JsonConvert.DeserializeObject<CarritoViewModel>(carritoJson) ?? new CarritoViewModel();
        }

        private ClienteViewModel ObtenerCliente()
        {
            var clienteJson = HttpContext.Session.GetString(ClienteSessionKey);

            if (string.IsNullOrEmpty(clienteJson))
            {
                return new ClienteViewModel();
            }

            return JsonConvert.DeserializeObject<ClienteViewModel>(clienteJson) ?? new ClienteViewModel();
        }
        
        private void GuardarCarrito(CarritoViewModel carrito)
        {
            var carritoJson = JsonConvert.SerializeObject(carrito);
            HttpContext.Session.SetString(CarritoSessionKey, carritoJson);
        }

        private void GuardarCliente(ClienteViewModel cliente)
        {
            var clienteJson = JsonConvert.SerializeObject(cliente);
            HttpContext.Session.SetString(ClienteSessionKey, clienteJson);
        }

        private void LimpiarCarrito2()
        {
            HttpContext.Session.Remove(CarritoSessionKey);
        }
    }

}
