using MySql.Data.MySqlClient; // Asegúrate de tener la referencia correcta

var builder = WebApplication.CreateBuilder(args);

// Obtener cadena de conexión desde appsettings.json
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
        builder.WithOrigins("*:8104")
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

// Conexión a MySQL y ejecución de consulta
using (var conexion = new MySqlConnection(conexionString))
{
    await conexion.OpenAsync(); // Abrir conexión

    var sql = "SELECT * FROM inmueble";
    var sentencia = new MySqlCommand(sql, conexion);

    using (var reader = await sentencia.ExecuteReaderAsync()) // Ejecución de la consulta
    {
        while (await reader.ReadAsync())
        {
            var resultado = reader.ToString(); // Obtener el valor de la consulta
            Console.WriteLine($"Resultado: {resultado}");
        }
    }

    await conexion.CloseAsync(); // Cerrar la conexión
}

// Ejecutar la aplicación
app.Run();
