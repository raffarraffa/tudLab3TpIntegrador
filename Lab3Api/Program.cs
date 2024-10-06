using Microsoft.EntityFrameworkCore;
using Lab3Api.Errors;
using Lab3Api.Models;
using Lab3Api.Repositories;


var builder = WebApplication.CreateBuilder(args);
// cinfiguracion puertos
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001", "http://*:5000", "https://*:5001");

// Registrar controladores en los servicios
builder.Services.AddControllers();

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
