using Microsoft.EntityFrameworkCore;
using Lab3Api.Errors;
using Lab3Api.Models;
using Lab3Api.Data;
using Lab3Api.Repositories;


var builder = WebApplication.CreateBuilder(args);
// Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("ConnectionProduction");
// configuracion db
#pragma warning disable CS8604
builder.Services.AddDbContext<ApiDbContext>(options => options.UseMySQL(connectionString));
#pragma warning restore CS8604
// servicios
builder.Services.AddScoped<RPropietario>();
builder.Services.AddScoped<IRepository<Propietario>, RPropietario>();
// cinfiguracion puertos
builder.WebHost.UseUrls("http://localhost:8104");

// Registrar controladores en los servicios
builder.Services.AddControllers();
// Configurar CORS
// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        //    builder.WithOrigins("http://localhost:8104", "https://localhost:8105")
        builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
    });
});
// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline HTTP para desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplicar CORS
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseRouting();
// error mildware
app.UseMiddleware<ErrorMiddleware>();
// router default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

// Iniciar la aplicación
app.Run();
