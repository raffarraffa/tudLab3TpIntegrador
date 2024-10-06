
var builder = WebApplication.CreateBuilder(args);

// cadena de conexión ->  appsettings.json
var conexionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registrar MySqlConnection en los servicios
builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(conexionString));

// Servicios adicionales
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configurar el uso de una URL específica
builder.WebHost.UseUrls("http://localhost:8104");

// Middleware para registrar las solicitudes y respuestas
app.Use(async (context, next) =>
{
    Console.WriteLine($"Req: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
    Console.WriteLine($"Res: {context.Response.StatusCode}");
});

// Aplicar CORS
app.UseCors("CorsPolicy");

// Habilitar controladores
app.UseAuthorization();
app.MapControllers();

// Ejecutar la aplicación
app.Run();
