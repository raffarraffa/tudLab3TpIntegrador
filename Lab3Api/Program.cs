using Microsoft.EntityFrameworkCore;
// jwt
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
// autenticacion
using Microsoft.IdentityModel.Tokens;

using Lab3Api.Errors;
using Lab3Api.Models;
using Lab3Api.Data;
using Lab3Api.Repositories;


// argumentos
var builder = WebApplication.CreateBuilder(args);

/** configuraciones  desde appsetings.json **/
// Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("ConnectionProduction");
// server
var server = builder.Configuration.GetSection("Servers");
// en points puerto 
var httpPort = server["http:Url"] ?? "http://localhost:5000";
var httpsPort = server["https:Url"] ?? "https://localhost:5001";
// jwt
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//secreto, emisor y audiencia  para token
var secret = jwtSettings["Secret"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

// configuracion db
#pragma warning disable CS8604
builder.Services.AddDbContext<ApiDbContext>(options => options.UseMySQL(connectionString));
#pragma warning restore CS8604
// Registrar repositorios
builder.Services.AddScoped<RPropietario>();
builder.Services.AddScoped<IRepository<Propietario>, RPropietario>();
// configuracion jwt
// builder.Services.AddAuthentication("Bearer");
// builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // emisor
            ValidateIssuer = true,
            //destino
            ValidateAudience = true,
            //expiracion
            ValidateLifetime = true,
            //firma
            ValidateIssuerSigningKey = true,
            //validaciones
            ValidIssuer = issuer,
            ValidAudience = audience,
#pragma warning disable CS8604
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
#pragma warning restore CS8604
        };
    });

// cinfiguracion puertos
//builder.WebHost.UseUrls("http://localhost:8104");

builder.WebHost.UseUrls(httpPort, httpsPort);


// Registrar controladores en los servicios
builder.Services.AddControllers();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        //    builder.WithOrigins(httpPort,httpsPort)
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
