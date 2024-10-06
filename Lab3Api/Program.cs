using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Lab3Api.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("ConnectionProduction");

// Agregar servicios al contenedor.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar InmobiliariaDbContext en los servicios
builder.Services.AddDbContext<InmobiliariaDbContext>(options =>
    options.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 4, 27)))
);

var app = builder.Build();

// Activa el enrutamiento
app.UseRouting();
// Rutas
app.MapGet("/", () => "¡Hola, mundodsdsdf!");
app.MapGet("/test-query", async (InmobiliariaDbContext dbContext) =>
{
    var direccion = string.Empty;

    using (var command = dbContext.Database.GetDbConnection().CreateCommand())
    {
        command.CommandText = "SELECT direccion FROM inmueble WHERE id = 1";
        dbContext.Database.OpenConnection();

        using (var result = await command.ExecuteReaderAsync())
        {
            if (await result.ReadAsync())
            {
                direccion = result.GetString(0);
            }
        }
    }

    if (!string.IsNullOrEmpty(direccion))
    {
        Console.WriteLine($"Resultado de la consulta: {direccion}");
        return Results.Ok(direccion);
    }
    else
    {
        Console.WriteLine("No se encontró ninguna dirección.");
        return Results.NotFound("No se encontró ninguna dirección.");
    }
});


// Configure el pipeline de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar redirección a HTTPS
//app.UseHttpsRedirection();

app.Run();
