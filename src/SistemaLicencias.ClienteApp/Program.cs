using NLog;
using NLog.Web;
using SistemaLicencias.ClienteApp.Services;

//var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
//logger.Debug("Iniciando aplicaci�n cliente");

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


    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Configurar HttpClient para consumir la API
    builder.Services.AddHttpClient<ILicenciasApiService, LicenciasApiService>()
        .ConfigureHttpClient((serviceProvider, client) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

    builder.Services.AddHttpClient<IMailingApiService, MailingApiService>()
    .ConfigureHttpClient((serviceProvider, client) =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var baseUrl = configuration["ApiSettings:BaseMailingUrl"];
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
    
    // Configurar Session
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = ".SistemaLicencias.Session";
    });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    // Habilitar Session
    app.UseSession();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    logger.Info("Aplicaci�n cliente iniciada correctamente");

    app.Run();


}
catch (Exception ex)
{
    logger.Error(ex, "La aplicaci�n cliente se detuvo debido a una excepci�n");
    throw;
}
finally
{
    LogManager.Shutdown();
}

