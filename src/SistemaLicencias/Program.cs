using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using NLog;
using NLog.Web;

//var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
NLog.GlobalDiagnosticsContext.Set("LogDirectory", logPath);

var logger = LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    // Configurar NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Agregar servicios al contenedor
    builder.Services.AddControllersWithViews();


    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configurar autenticación con cookies
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Auth/Login";
            options.LogoutPath = "/Auth/Logout";
            options.AccessDeniedPath = "/Auth/Login";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

    builder.Services.AddAuthorization();

    // Registrar DapperContext
    builder.Services.AddSingleton<DapperContext>();

    // Registrar Repositories
    builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
    builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
    builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
    builder.Services.AddScoped<IDetallePedidoRepository, DetallePedidoRepository>();
    builder.Services.AddScoped<ILicenciaRepository, LicenciaRepository>();
    builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

    // Registrar Services
    builder.Services.AddScoped<IProductoService, ProductoService>();
    builder.Services.AddScoped<IPedidoService, PedidoService>();
    builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();
    builder.Services.AddScoped<IEmailService, EmailService>();

    // Registrar Niubiz Service con HttpClient
    builder.Services.AddHttpClient<INiubizService, NiubizService>();

    // Registrar Services de Infrastructure
    builder.Services.AddScoped<IEmailService, EmailService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowClientApp", policy =>
        {
            policy.WithOrigins("https://localhost:7107/", "http://localhost:7107/")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowClientApp");

    app.UseHttpsRedirection();

    app.UseAuthentication();  // Primero autenticación
    app.UseAuthorization();   // Luego autorización

    app.MapControllers();

    app.Run();


}
catch (Exception ex)
{
    logger.Error(ex, "La aplicación se detuvo debido a una excepción");
    throw;
}
finally
{
    LogManager.Shutdown();
}

